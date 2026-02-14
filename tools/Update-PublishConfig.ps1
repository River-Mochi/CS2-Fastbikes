# File: tools/Update-PublishConfig.ps1
# Sync PublishConfiguration.xml ModVersion to csproj Version.
# Optional: Left-align ONLY LongDescription + ChangeLog inner text for Paradox formatting.

param(
  [Parameter(Mandatory = $true)][string]$Path,
  [Parameter(Mandatory = $true)][string]$Version,
  [ValidateSet('crlf','lf')][string]$Eol = 'crlf',
  [switch]$LeftAlignBlocks
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $Path)) {
  throw "PublishConfiguration.xml not found: $Path"
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$original  = [System.IO.File]::ReadAllText($Path, $utf8NoBom)

# Safety: never write to an empty file.
if ([string]::IsNullOrWhiteSpace($original)) {
  throw "Refusing to update because file is empty: $Path (restore it first)"
}

function Normalize-Eol([string]$s, [string]$eolKind) {
  # Collapse all newline variants to LF first, then expand to CRLF if requested.
  $s = $s.Replace("`r`n", "`n").Replace("`r", "`n")
  $s = $s.Replace([char]0x85,  "`n")   # NEL
  $s = $s.Replace([char]0x2028, "`n")  # LS
  $s = $s.Replace([char]0x2029, "`n")  # PS

  if ($eolKind -eq 'crlf') {
    $s = $s.Replace("`n", "`r`n")
  }
  return $s
}

function LeftAlignInnerBlock([string]$s, [string]$tagName) {
  $opts = [System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor
          [System.Text.RegularExpressions.RegexOptions]::Singleline

  $rx = [System.Text.RegularExpressions.Regex]::new(
    "(<$tagName\b[^>]*>)(.*?)(</$tagName>)",
    $opts
  )

  if (-not $rx.IsMatch($s)) { return $s }

  return $rx.Replace($s, { param($m)
    $open  = $m.Groups[1].Value
    $inner = $m.Groups[2].Value
    $close = $m.Groups[3].Value

    # Strip leading spaces/tabs per line inside the block; keep blank lines.
    $inner2 = [System.Text.RegularExpressions.Regex]::Replace($inner, '(?m)^[\t ]+', '')
    $open + $inner2 + $close
  }, 1)
}

# Normalize early so mixed EOL doesn't propagate.
$text = Normalize-Eol $original $Eol

# Optional: only affects inner text of these blocks.
if ($LeftAlignBlocks) {
  $text = LeftAlignInnerBlock $text 'LongDescription'
  $text = LeftAlignInnerBlock $text 'ChangeLog'
}

# Replace whatever is inside ModVersion Value="...".
$rxMod = [System.Text.RegularExpressions.Regex]::new(
  '(<ModVersion\b[^>]*\bValue=")[^"]*(")',
  [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
)

if ($rxMod.IsMatch($text)) {
  $text = $rxMod.Replace($text, { param($m)
    $m.Groups[1].Value + $Version + $m.Groups[2].Value
  }, 1)
} else {
  throw "Could not find <ModVersion ... Value=""..."" ...> in: $Path"
}

# Final normalize after edits.
$text = Normalize-Eol $text $Eol

# Sanity checks: refuse to write MIXED when CRLF requested.
if ($Eol -eq 'crlf') {
  if ($text -match "(?<!`r)`n") { throw "Internal error: bare LF exists before write (would create MIXED)" }
  if ($text -match "`r(?!`n)")  { throw "Internal error: bare CR exists before write (would create MIXED)" }
}

# If no change, do nothing (no .bak churn).
if ($text -eq $original) {
  Write-Host ("No change needed: {0} (ModVersion already {1})." -f (Split-Path -Leaf $Path), $Version)
  exit 0
}

# Backup + atomic write.
$bak = "$Path.bak"
[System.IO.File]::WriteAllText($bak, $original, $utf8NoBom)

$tmp = "$Path.tmp"
[System.IO.File]::WriteAllText($tmp, $text, $utf8NoBom)
Move-Item -Force -LiteralPath $tmp -Destination $Path

# Print backup path as "RepoFolder\Properties\PublishConfiguration.xml.bak"
$repoRoot = Split-Path -Parent $PSScriptRoot
$repoName = Split-Path -Leaf $repoRoot
$bakTail  = $bak.Substring($repoRoot.Length).TrimStart('\')
$bakShort = "$repoName\$bakTail"

Write-Host ("ModVersion updated to [{0}] in: {1} (EOL={2}, LeftAlignBlocks={3}).`nBACKUP in: {4}" -f `
  $Version, (Split-Path -Leaf $Path), $Eol, $LeftAlignBlocks.IsPresent, $bakShort)
