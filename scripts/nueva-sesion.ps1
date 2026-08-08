# Crea una nota de sesion en el vault de Obsidian (docs/sesiones/) con nombre
# YYYY-MM-DD_tema, setea las properties del frontmatter y arrastra los
# pendientes abiertos de la sesion anterior. Tambien anota la entrada en BITACORA.
#
# Uso:
#   .\scripts\nueva-sesion.ps1 -Tema "Habilitacion CLI + Obsidian"
#   .\scripts\nueva-sesion.ps1 -Tema "Presupuesto" -Modulo "Presupuesto" -Estado "En curso"
#
# Nota: el fuente es ASCII puro; los caracteres acentuados y el emoji del titulo
# se construyen en runtime para evitar problemas de codificacion con PowerShell 5.1.

[CmdletBinding()]
param(
    [string]$Tema,
    [string]$Modulo = 'General',
    [string]$Estado = 'En curso',
    [switch]$NoOpen
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$vaultDir = Join-Path $repoRoot 'docs'
$sessionsDir = Join-Path $vaultDir 'sesiones'
$templateFile = Join-Path $vaultDir 'templates\Sesion.md'
$bitacoraFile = Join-Path $vaultDir 'BITACORA.md'

if (-not (Test-Path -LiteralPath $templateFile)) {
    throw "Template no encontrado: $templateFile"
}
if (-not (Test-Path -LiteralPath $bitacoraFile)) {
    throw "BITACORA no encontrada: $bitacoraFile"
}
if (-not (Test-Path -LiteralPath $sessionsDir)) {
    New-Item -ItemType Directory -Path $sessionsDir -Force | Out-Null
    Write-Host "[DIR] Creada carpeta de sesiones: $sessionsDir"
}

if (-not $Tema) {
    $Tema = Read-Host 'Tema de la sesion: '
    if (-not $Tema) { throw 'Necesitas un tema para crear la sesion.' }
}

# Caracteres no ASCII que se insertan en las notas (fuente del script es ASCII).
$accentO = [string][char]0x00F3
$dash = [string][char]0x2014
$sesionWord = 'Sesi' + $accentO + 'n'
$clip = [char]::ConvertFromUtf32(0x1F4CB)

# Slug del tema (sin acentos, minusculas, guiones) para el nombre de archivo.
$slug = $Tema.Trim() -replace '\s+', '-' -replace '[^\p{L}\p{N}_-]', '' -replace '-+', '-'
$slug = $slug.Normalize([System.Text.NormalizationForm]::FormD)
$slug = [regex]::Replace($slug, '[^\p{IsBasicLatin}]', '')
$slug = $slug.Trim('-').ToLowerInvariant()
if (-not $slug) { $slug = 'sesion' }

$now = Get-Date
$iso = $now.ToString('yyyy-MM-dd')
$nombre = '{0}_{1}' -f $iso, $slug
$relPath = 'sesiones/{0}.md' -f $nombre
$target = Join-Path $sessionsDir ($nombre + '.md')

if (Test-Path -LiteralPath $target) {
    throw "Ya existe una nota de sesion: $relPath"
}

# Construir el contenido a partir del template (lectura UTF-8 explicita).
$content = Get-Content -LiteralPath $templateFile -Raw -Encoding UTF8
$content = $content.Replace('fecha: ""', "fecha: `"$iso`"")
$content = $content.Replace('modulo: ""', "modulo: `"$Modulo`"")
$content = $content.Replace('estado: ""', "estado: `"$Estado`"")
$oldHead = "# $clip $sesionWord"
$newHead = "# $clip $sesionWord $dash $Tema"
$content = $content.Replace($oldHead, $newHead)

$enc = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($target, $content.TrimEnd() + "`n", $enc)
Write-Host "[OK] Nota creada: $relPath"

# Arrastrar pendientes abiertos de la sesion anterior (la mas reciente en sesiones/).
$last = Get-ChildItem -LiteralPath $sessionsDir -Filter '*.md' -File |
    Where-Object { $_.Name -ne ($nombre + '.md') } |
    Sort-Object Name -Descending |
    Select-Object -First 1

if ($last) {
    $pendientes = @()
    foreach ($l in (Get-Content -LiteralPath $last.FullName -Encoding UTF8)) {
        if ($l -match '^\s*-\s*\[\s\]') {
            $pendientes += $l.Trim()
        }
    }
    if ($pendientes.Count -gt 0) {
        $arrHead = "`n## Arrastrados de la sesi" + $accentO + "n anterior ($($last.BaseName))`n`n"
        $block = $arrHead + ($pendientes -join "`n") + "`n"
        [System.IO.File]::AppendAllText($target, $block, $enc)
        Write-Host ("[OK] {0} pendiente(s) arrastrados desde {1}" -f $pendientes.Count, $last.Name)
    } else {
        Write-Host "[OK] Sin pendientes abiertos en la sesion anterior ($($last.Name))."
    }
} else {
    Write-Host '[OK] Primera sesion; sin arrastre de pendientes.'
}

# Anotar la entrada en BITACORA (numero de sesion siguiente).
$bitacoraContent = Get-Content -LiteralPath $bitacoraFile -Raw -Encoding UTF8
$pattern = '(?m)^##\s*Sesi' + $accentO + 'n\s+(\d+)'
$nums = @([regex]::Matches($bitacoraContent, $pattern) | ForEach-Object { [int]$_.Groups[1].Value })
$nextNum = 1
if ($nums.Count -gt 0) { $nextNum = ($nums | Measure-Object -Maximum).Maximum + 1 }

$fechaTxt = $now.ToString('dddd dd MMMM yyyy', [System.Globalization.CultureInfo]::GetCultureInfo('es-ES'))
$notaLine = 'Nota de sesi' + $accentO + 'n'
$bitLine = "`n## $sesionWord $nextNum $dash ${iso}: $Tema`n`n${notaLine}: [[$nombre]]`n"
[System.IO.File]::AppendAllText($bitacoraFile, $bitLine, $enc)
Write-Host "[OK] BITACORA actualizada (Sesion $nextNum)."

Write-Host ''
Write-Host '=== Resumen ==='
Write-Host "Nota:   $relPath"
Write-Host "Fecha:  $fechaTxt"
Write-Host "Tema:   $Tema  |  Modulo: $Modulo  |  Estado: $Estado"

if (-not $NoOpen) {
    $uriFile = [uri]::EscapeDataString(($relPath -replace '\.md$', ''))
    Start-Process ("obsidian://open?vault=docs&file=" + $uriFile) | Out-Null
    Write-Host '[OPEN] Abriendo la nota en Obsidian...'
}
