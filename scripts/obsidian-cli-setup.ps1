[CmdletBinding()]
param(
    [string]$ObsidianDir,
    [switch]$SkipPath,
    [switch]$SkipToggle,
    [switch]$OnlyVerify,
    [switch]$Start
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName UIAutomationClient -ErrorAction SilentlyContinue
Add-Type -AssemblyName UIAutomationTypes -ErrorAction SilentlyContinue
Add-Type -AssemblyName System.Windows.Forms -ErrorAction SilentlyContinue
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class ClickHelper {
  [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
  [DllImport("user32.dll")] public static extern void mouse_event(uint f, uint dx, uint dy, uint d, UIntPtr e);
  public static void Click(int x, int y) {
    SetCursorPos(x, y);
    mouse_event(0x0002, 0, 0, 0, UIntPtr.Zero);
    mouse_event(0x0004, 0, 0, 0, UIntPtr.Zero);
  }
}
"@

function Find-ObsidianDir {
    param([string]$Hint)
    if ($Hint -and (Test-Path (Join-Path $Hint 'Obsidian.com'))) {
        return $Hint
    }
    $candidates = @(
        'C:\Program Files\Obsidian',
        'C:\Program Files (x86)\Obsidian',
        (Join-Path $env:LOCALAPPDATA 'Obsidian'),
        (Join-Path $env:LOCALAPPDATA 'Programs\Obsidian')
    )
    foreach ($c in $candidates) {
        if (Test-Path (Join-Path $c 'Obsidian.com')) {
            return $c
        }
    }
    $uninst = Get-ItemProperty 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*', 'HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*' -ErrorAction SilentlyContinue
    $entry = $uninst | Where-Object { $_.DisplayName -like '*Obsidian*' -and $_.InstallLocation } | Select-Object -First 1
    if ($entry -and (Test-Path (Join-Path $entry.InstallLocation 'Obsidian.com'))) {
        return $entry.InstallLocation
    }
    throw 'Obsidian no encontrado. Pasa el directorio con -ObsidianDir.'
}

function Add-ObsidianToPath {
    param([string]$Dir)
    $userPath = [Environment]::GetEnvironmentVariable('Path', 'User')
    if (-not $userPath) { $userPath = '' }
    $parts = $userPath -split ';' | Where-Object { $_ }
    if ($parts -contains $Dir) {
        Write-Host "[PATH] Ya esta registrado: $Dir"
        return
    }
    $newPath = ($parts + $Dir) -join ';'
    [Environment]::SetEnvironmentVariable('Path', $newPath, 'User')
    Write-Host "[PATH] Agregado al PATH de usuario: $Dir"
    Write-Host "[PATH] Nota: abre una terminal NUEVA para usar el comando 'obsidian'."
}

function Get-CliStatus {
    param([string]$ComPath)
    $out = & $ComPath version 2>&1 | Out-String
    $text = $out.Trim()
    if ($text -match 'not enabled') {
        return @{ State = 'NotEnabled'; Text = $text }
    }
    if ($text -match 'unable to find|is it running|could not connect|no est. en ejecuci') {
        return @{ State = 'AppNotRunning'; Text = $text }
    }
    if ($text -match '\d+\.\d+\.\d+') {
        return @{ State = 'Enabled'; Text = $text }
    }
    return @{ State = 'Unknown'; Text = $text }
}

function Start-ObsidianApp {
    param([string]$Exe)
    Start-Process -FilePath $Exe | Out-Null
    Write-Host "[APP] Lanzando Obsidian..."
    $deadline = (Get-Date).AddSeconds(60)
    do {
        Start-Sleep -Milliseconds 1000
        $proc = Get-Process -Name 'Obsidian' -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
    } while (-not $proc -and (Get-Date) -lt $deadline)
    if (-not $proc) {
        throw 'Obsidian se lanzo pero no se detecto su ventana.'
    }
    Start-Sleep -Seconds 6
    return $proc.Id
}

function Get-ObsidianMainWindow {
    return (Get-Process -Name 'Obsidian' -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1)
}

function Get-ObsidianSettingsWindow {
    param([int]$WinPid)
    $root = [System.Windows.Automation.AutomationElement]::RootElement
    $wins = $root.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($w in $wins) {
        if ($w.Current.ProcessId -eq $WinPid -and ($w.Current.Name -like 'Preferencias*' -or $w.Current.Name -like 'Settings*')) {
            return $w
        }
    }
    return $null
}

function Open-ObsidianSettings {
    param([int]$WinPid)
    $wshell = New-Object -ComObject WScript.Shell
    $win = Get-ObsidianSettingsWindow -WinPid $WinPid
    if ($win) { return $win }
    if (-not $wshell.AppActivate($WinPid)) {
        Write-Warning '[SETTINGS] No se pudo enfocar Obsidian.'
        return $null
    }
    Start-Sleep -Milliseconds 800
    $wshell.SendKeys('^,')
    $deadline = (Get-Date).AddSeconds(15)
    do {
        Start-Sleep -Milliseconds 500
        $win = Get-ObsidianSettingsWindow -WinPid $WinPid
    } while (-not $win -and (Get-Date) -lt $deadline)
    return $win
}

function Clear-SettingsSearch {
    param($sw)
    $edit = $sw.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
    if (-not $edit) { return }
    try {
        $vp = $edit.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $vp.SetValue('')
    } catch {}
}

function Find-SettingsNavItem {
    param($sw, [string]$Label)
    $nav = $sw.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ClassNameProperty, 'vertical-tab-header')))
    if (-not $nav) { return $null }
    $all = $nav.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($a in $all) {
        if ($a.Current.ClassName -like 'vertical-tab-nav-item*') {
            $t = $a.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)))
            if ($t -and $t.Current.Name -eq $Label) {
                return $a
            }
        }
    }
    return $null
}

function Find-CliSettingRow {
    param($sw)
    $rows = $sw.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ClassNameProperty, 'setting-item mod-toggle')))
    foreach ($r in $rows) {
        $texts = $r.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)))
        foreach ($t in $texts) {
            if ($t.Current.Name -match 'interfaz.*comandos|command line interface|command line') {
                return $r
            }
        }
    }
    return $null
}

function Click-Element {
    param($el)
    $r = $el.Current.BoundingRectangle
    if ($r.Width -lt 1 -or $r.Height -lt 1) {
        return $false
    }
    [ClickHelper]::Click([int]($r.X + $r.Width / 2), [int]($r.Y + $r.Height / 2))
    return $true
}

function Invoke-CliToggle {
    param([int]$WinPid)
    $wshell = New-Object -ComObject WScript.Shell
    $sw = Open-ObsidianSettings -WinPid $WinPid
    if (-not $sw) {
        Write-Warning '[TOGGLE] No se pudo abrir Settings.'
        return $false
    }
    Start-Sleep -Milliseconds 800
    Clear-SettingsSearch $sw | Out-Null

    $navItem = Find-SettingsNavItem $sw 'Acerca de'
    if (-not $navItem) { $navItem = Find-SettingsNavItem $sw 'About' }
    if ($navItem) {
        Click-Element $navItem | Out-Null
        Start-Sleep -Milliseconds 900
    }

    $row = Find-CliSettingRow $sw
    if (-not $row) {
        Write-Warning '[TOGGLE] No se encontro el control "Interfaz de linea de comandos" en Settings.'
        return $false
    }

    $checkbox = $row.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ClassNameProperty, 'checkbox-container')))
    if (-not $checkbox) {
        Write-Warning '[TOGGLE] No se encontro el interruptor.'
        return $false
    }

    if ($checkbox.Current.ClassName -match 'is-enabled') {
        Write-Host '[TOGGLE] El CLI ya estaba activado en Settings.'
        return $true
    }

    try {
        $sip = $checkbox.GetCurrentPattern([System.Windows.Automation.ScrollItemPattern]::Pattern)
        $sip.ScrollIntoView()
        Start-Sleep -Milliseconds 600
        $checkbox.SetFocus()
    } catch {
        Write-Warning "[TOGGLE] No se pudo enfocar el interruptor: $($_.Exception.Message)"
        return $false
    }
    Start-Sleep -Milliseconds 400
    $wshell.AppActivate($sw.Current.NativeWindowHandle) | Out-Null
    Start-Sleep -Milliseconds 200
    $wshell.SendKeys(' ')
    Start-Sleep -Milliseconds 1500
    $wshell.SendKeys('{ESC}')
    Start-Sleep -Milliseconds 300
    return $true
}

Write-Host '=== Obsidian CLI Setup ==='
$dir = Find-ObsidianDir -Hint $ObsidianDir
$com = Join-Path $dir 'Obsidian.com'
$exe = Join-Path $dir 'Obsidian.exe'
Write-Host "[DIR] $dir"

if (-not $SkipPath) {
    Add-ObsidianToPath -Dir $dir
}

$status = Get-CliStatus -ComPath $com
Write-Host "[CLI] Estado: $($status.State) -- $($status.Text)"

if ($Start) {
    if ($status.State -eq 'AppNotRunning' -and (Test-Path $exe)) {
        Start-ObsidianApp -Exe $exe | Out-Null
        Start-Sleep -Seconds 3
        $status = Get-CliStatus -ComPath $com
    }
    Start-Process "obsidian://open?vault=docs" | Out-Null
    Start-Sleep -Seconds 2
    Write-Host "[START] Solicitado abrir el vault 'docs'."
    if ($status.State -eq 'Enabled') {
        $vaults = & $com vaults 2>&1 | Out-String
        if ($vaults) { Write-Host "[START] vaults:`n$($vaults.Trim())" }
        $read = & $com read file='INDEX' 2>&1 | Out-String
        if ($read -match 'not found|error|Error') {
            Write-Host '[START] INDEX no resuelto; enfoca el vault docs en Obsidian.'
        } else {
            $firstLine = (($read.Trim()) -split '\r?\n')[0]
            Write-Host "[START] read INDEX -> $firstLine"
        }
    } else {
        Write-Host "[START] CLI no disponible (estado: $($status.State)). Abre Obsidian y activa el CLI."
    }
    Write-Host '=== Fin (modo Start) ==='
    exit 0
}

if ($status.State -eq 'AppNotRunning' -and -not $OnlyVerify) {
    if (Test-Path $exe) {
        $mainPid = Start-ObsidianApp -Exe $exe
    } else {
        throw "Obsidian.exe no existe en $dir; abre Obsidian manualmente."
    }
    $status = Get-CliStatus -ComPath $com
    Write-Host "[CLI] Estado tras lanzar: $($status.State)"
}

if ($status.State -eq 'NotEnabled' -and -not $SkipToggle -and -not $OnlyVerify) {
    $win = Get-ObsidianMainWindow
    if ($win) {
        Write-Host '[TOGGLE] Activando el CLI en Settings (Acerca de > Interfaz de linea de comandos)...'
        Invoke-CliToggle -WinPid $win.Id | Out-Null
        Start-Sleep -Seconds 3
        $status = Get-CliStatus -ComPath $com
        Write-Host "[CLI] Estado tras toggle: $($status.State) -- $($status.Text)"
    } else {
        Write-Warning '[TOGGLE] Obsidian no tiene ventana visible; no se puede automatizar el toggle.'
    }
}

Write-Host ''
Write-Host '=== Verificacion final ==='
if ($status.State -eq 'Enabled') {
    Write-Host '[OK] CLI habilitado.'
    $v = & $com version 2>&1 | Out-String
    Write-Host "[OK] version: $($v.Trim())"
    $vaults = & $com vaults 2>&1 | Out-String
    if ($vaults) { Write-Host "[OK] vaults:`n$($vaults.Trim())" }
    $read = & $com read file='INDEX' 2>&1 | Out-String
    if ($read -match 'not found|error|Error') {
        Write-Host '[NOTA] No se pudo leer INDEX; revisa que el vault correcto este enfocado.'
    } else {
        $firstLine = (($read.Trim()) -split '\r?\n')[0]
        Write-Host "[OK] read INDEX -> $firstLine"
    }
} else {
    Write-Host "[!] El CLI sigue en estado: $($status.State)"
    Write-Host ''
    Write-Host 'Pasos manuales (una sola vez):'
    Write-Host '  1. Abre Obsidian -> Settings (Ctrl+,) -> Acerca de.'
    Write-Host '  2. Activa el interruptor "Interfaz de linea de comandos".'
    Write-Host '  3. Sigue las instrucciones en pantalla para agregar el CLI al PATH.'
    Write-Host '  4. Verifica en una terminal nueva con:  obsidian version'
}
Write-Host '=== Fin ==='
