param(
    [Parameter(Mandatory = $true)]
    [string]$Owner,

    [Parameter(Mandatory = $true)]
    [string]$Repo
)

$ErrorActionPreference = "Stop"

function New-GitHubLabel {
    param(
        [string]$Name,
        [string]$Color,
        [string]$Description
    )

    gh api "repos/$Owner/$Repo/labels" `
        --method POST `
        -f name="$Name" `
        -f color="$Color" `
        -f description="$Description" 2>$null
}

function New-GitHubMilestone {
    param(
        [string]$Title,
        [string]$DueDateIso,
        [string]$Description
    )

    gh api "repos/$Owner/$Repo/milestones" `
        --method POST `
        -f title="$Title" `
        -f due_on="$DueDateIso" `
        -f description="$Description" 2>$null
}

Write-Host "Creando labels base..."
New-GitHubLabel -Name "type:feature" -Color "0E8A16" -Description "Funcionalidad de producto"
New-GitHubLabel -Name "type:infra" -Color "1D76DB" -Description "Infraestructura o plataforma"
New-GitHubLabel -Name "type:testing" -Color "5319E7" -Description "Testing strategy, tests, quality gates"
New-GitHubLabel -Name "priority:p0" -Color "B60205" -Description "Prioridad critica"
New-GitHubLabel -Name "area:api" -Color "FBCA04" -Description "Capa API"
New-GitHubLabel -Name "area:domain" -Color "D4C5F9" -Description "Capa dominio"
New-GitHubLabel -Name "area:data" -Color "C2E0C6" -Description "Persistencia y datos"
New-GitHubLabel -Name "area:quality" -Color "006B75" -Description "Calidad, testing, observabilidad"
New-GitHubLabel -Name "area:devops" -Color "E99695" -Description "CI/CD, Docker, entrega"

Write-Host "Creando milestones P0..."
New-GitHubMilestone `
    -Title "P0-M1 Testing Foundation" `
    -DueDateIso "2026-05-10T23:59:59Z" `
    -Description "Arquitectura base + integration harness + flujos iniciales"

New-GitHubMilestone `
    -Title "P0-M2 Core Learning Flows" `
    -DueDateIso "2026-05-24T23:59:59Z" `
    -Description "Ruta, lecciones, materiales y sesiones"

New-GitHubMilestone `
    -Title "P0-M3 Delivery Hardening" `
    -DueDateIso "2026-06-07T23:59:59Z" `
    -Description "Feedback parental, progreso agregado, Docker y CI"

Write-Host "Listo. Ahora crea los issues desde:"
Write-Host "docs/planning/github-milestones-and-issues-p0.md"

