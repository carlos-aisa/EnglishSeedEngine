param(
    [Parameter(Mandatory = $true)]
    [string]$Owner,

    [Parameter(Mandatory = $true)]
    [string]$Repo
)

$ErrorActionPreference = "Stop"

function Assert-GitHubCli {
    $null = gh --version
}

function Assert-RepositoryExists {
    gh api "repos/$Owner/$Repo" --silent 2>$null
}

function Upsert-GitHubLabel {
    param(
        [string]$Name,
        [string]$Color,
        [string]$Description
    )

    $encodedName = [System.Uri]::EscapeDataString($Name)

    gh api "repos/$Owner/$Repo/labels/$encodedName" --silent 2>$null

    if ($LASTEXITCODE -eq 0) {
        gh api "repos/$Owner/$Repo/labels/$encodedName" `
            --method PATCH `
            -f new_name="$Name" `
            -f color="$Color" `
            -f description="$Description" >$null
        Write-Host "Label actualizado: $Name"
        return
    }

    gh api "repos/$Owner/$Repo/labels" `
        --method POST `
        -f name="$Name" `
        -f color="$Color" `
        -f description="$Description" >$null
    Write-Host "Label creado: $Name"
}

function Upsert-GitHubMilestone {
    param(
        [string]$Title,
        [string]$DueDateIso,
        [string]$Description
    )

    $milestonesJson = gh api "repos/$Owner/$Repo/milestones?state=all&per_page=100" 2>$null
    $milestones = $milestonesJson | ConvertFrom-Json
    $existingMilestone = $milestones | Where-Object { $_.title -eq $Title } | Select-Object -First 1

    if ($null -ne $existingMilestone) {
        $milestoneNumber = $existingMilestone.number
        gh api "repos/$Owner/$Repo/milestones/$milestoneNumber" `
            --method PATCH `
            -f title="$Title" `
            -f due_on="$DueDateIso" `
            -f description="$Description" >$null
        Write-Host "Milestone actualizado: $Title"
        return
    }

    gh api "repos/$Owner/$Repo/milestones" `
        --method POST `
        -f title="$Title" `
        -f due_on="$DueDateIso" `
        -f description="$Description" >$null
    Write-Host "Milestone creado: $Title"
}

Assert-GitHubCli
Assert-RepositoryExists

Write-Host "Creando labels base..."
Upsert-GitHubLabel -Name "type:feature" -Color "0E8A16" -Description "Funcionalidad de producto"
Upsert-GitHubLabel -Name "type:infra" -Color "1D76DB" -Description "Infraestructura o plataforma"
Upsert-GitHubLabel -Name "type:testing" -Color "5319E7" -Description "Testing strategy, tests, quality gates"
Upsert-GitHubLabel -Name "priority:p0" -Color "B60205" -Description "Prioridad critica"
Upsert-GitHubLabel -Name "area:api" -Color "FBCA04" -Description "Capa API"
Upsert-GitHubLabel -Name "area:domain" -Color "D4C5F9" -Description "Capa dominio"
Upsert-GitHubLabel -Name "area:data" -Color "C2E0C6" -Description "Persistencia y datos"
Upsert-GitHubLabel -Name "area:quality" -Color "006B75" -Description "Calidad, testing, observabilidad"
Upsert-GitHubLabel -Name "area:devops" -Color "E99695" -Description "CI/CD, Docker, entrega"

Write-Host "Creando milestones P0..."
Upsert-GitHubMilestone `
    -Title "P0-M1 Testing Foundation" `
    -DueDateIso "2026-05-10T23:59:59Z" `
    -Description "Arquitectura base + integration harness + flujos iniciales"

Upsert-GitHubMilestone `
    -Title "P0-M2 Core Learning Flows" `
    -DueDateIso "2026-05-24T23:59:59Z" `
    -Description "Ruta, lecciones, materiales y sesiones"

Upsert-GitHubMilestone `
    -Title "P0-M3 Delivery Hardening" `
    -DueDateIso "2026-06-07T23:59:59Z" `
    -Description "Feedback parental, progreso agregado, Docker y CI"

Write-Host "Listo. Ahora crea los issues desde:"
Write-Host "docs/planning/github-milestones-and-issues-p0.en.md"
