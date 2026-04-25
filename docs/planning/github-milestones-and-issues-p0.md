# EnglishSeed Engine - P0 GitHub Milestones + Issues

Este archivo te deja un backlog P0 listo para copiar en GitHub con foco en testing y calidad.

## Milestones (sugeridos)

1. `P0-M1 Testing Foundation`  
   Fecha objetivo: `2026-05-10`  
   Objetivo: base de arquitectura + harness de integration testing + primeros flujos core.
2. `P0-M2 Core Learning Flows`  
   Fecha objetivo: `2026-05-24`  
   Objetivo: ruta, lecciones, material y sesiones de practica con calidad de API.
3. `P0-M3 Delivery Hardening`  
   Fecha objetivo: `2026-06-07`  
   Objetivo: feedback parental, progreso agregado, Docker y CI.

## Labels recomendadas

`type:feature`, `type:infra`, `type:testing`, `priority:p0`, `area:api`, `area:domain`, `area:data`, `area:quality`, `area:devops`

## Definicion de Done comun (copiar en cada issue)

```md
## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

## Issues P0 (copiar y pegar)

### 1) INF-001 - Bootstrap solucion backend modular

- **Title:** `INF-001 Bootstrap solution and modular backend skeleton`
- **Labels:** `type:infra`, `priority:p0`, `area:api`, `area:domain`, `area:data`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## Contexto
Necesitamos una base limpia y mantenible para trabajar testing-first sin friccion.

## Alcance
- Crear solucion .NET 8:
  - src/EnglishSeedEngine.Api
  - src/EnglishSeedEngine.Application
  - src/EnglishSeedEngine.Domain
  - src/EnglishSeedEngine.Infrastructure
  - tests/EnglishSeedEngine.UnitTests
  - tests/EnglishSeedEngine.IntegrationTests
- Configurar dependencias por capas.
- Configurar EF Core + DbContext inicial.
- Health endpoint basico (`/health`).

## Criterios de aceptacion
- La solucion compila (`dotnet build`).
- La API arranca y responde `/health`.
- Arquitectura base documentada en README tecnico.

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 2) INF-002 - Harness de integration testing

- **Title:** `INF-002 Integration test harness with Testcontainers + Respawn`
- **Labels:** `type:testing`, `priority:p0`, `area:quality`, `area:data`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## Contexto
La prioridad absoluta del proyecto es Testing y Calidad.

## Alcance
- Configurar `WebApplicationFactory`.
- Configurar `Testcontainers` para PostgreSQL real.
- Configurar `Respawn` para limpiar estado entre tests.
- Crear clase base para tests de API.
- Anadir convenciones de naming y estructura de tests.

## Criterios de aceptacion
- Se puede ejecutar `dotnet test` localmente con DB real en contenedor.
- Los tests se ejecutan aislados y repetibles.
- El suite base tarda menos de 60 segundos en entorno local.

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 3) US-001 - Registro de alumno

- **Title:** `US-001 Register student profile`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:data`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## Historia
Como padre/madre quiero registrar un alumno para iniciar su plan de aprendizaje.

## Endpoint
- POST /students
- GET /students/{id}

## Criterios de aceptacion
- Crea alumno con nombre, edad y objetivo.
- Valida payload invalido (400).
- Detecta duplicado por email tutor (409).

## Tests de integracion
- CreateStudent_Returns201
- CreateStudent_InvalidPayload_Returns400
- CreateStudent_DuplicateTutorEmail_Returns409

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 4) US-002 - Diagnostico inicial de nivel

- **Title:** `US-002 Initial assessment and CEFR approximation`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## Historia
Como sistema quiero evaluar nivel inicial para personalizar aprendizaje.

## Endpoint
- POST /students/{id}/assessments/initial
- GET /students/{id}/level

## Criterios de aceptacion
- Persistencia del diagnostico.
- Clasificacion CEFR aproximada (A1/A2/B1).
- 404 para alumno inexistente.

## Tests de integracion
- SubmitInitialAssessment_AssignsLevel
- SubmitInitialAssessment_StudentNotFound_Returns404
- SubmitInitialAssessment_InvalidAnswers_Returns400

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 5) US-003 - Ruta personalizada de 4 semanas

- **Title:** `US-003 Create 4-week personalized learning plan`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## Historia
Como padre/madre quiero una ruta de aprendizaje personalizada para 4 semanas.

## Endpoint
- POST /students/{id}/learning-plans
- GET /learning-plans/{id}

## Criterios de aceptacion
- Crea objetivos semanales segun nivel y objetivo.
- No permite crear plan sin diagnostico previo (422).
- Devuelve estado actual del plan.

## Tests de integracion
- CreatePlan_FromAssessment_Returns201
- CreatePlan_WithoutAssessment_Returns422
- GetPlan_NotFound_Returns404

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 6) US-004 - Crear siguiente leccion

- **Title:** `US-004 Generate next lesson from active plan`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## Historia
Como sistema quiero crear la siguiente leccion para ejecutar el plan.

## Endpoint
- POST /learning-plans/{id}/lessons:next
- GET /lessons/{id}

## Criterios de aceptacion
- Crea leccion con dificultad objetivo y foco semanal.
- No crea leccion si el plan esta completado (409).
- 404 si el plan no existe.

## Tests de integracion
- CreateNextLesson_Returns201
- CreateNextLesson_PlanCompleted_Returns409
- CreateNextLesson_PlanNotFound_Returns404

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 7) US-005 - Generar material y ejercicios

- **Title:** `US-005 Generate lesson materials and exercises`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:quality`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## Historia
Como alumno quiero material de leccion para practicar.

## Endpoint
- POST /lessons/{id}/materials:generate
- GET /lessons/{id}/materials

## Criterios de aceptacion
- Genera vocabulario, frases y ejercicios (`cloze`, `translation`, `dictation`).
- Versiona y persiste el material.
- Gestiona errores de proveedor IA (503 controlado).

## Tests de integracion
- GenerateMaterials_Returns201
- GenerateMaterials_LessonNotFound_Returns404
- GenerateMaterials_AiProviderFailure_Returns503

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 8) US-006 - Sesion de practica y resultado

- **Title:** `US-006 Practice session scoring and weak points`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## Historia
Como alumno quiero enviar respuestas y obtener un resultado accionable.

## Endpoint
- POST /practice-sessions
- POST /practice-sessions/{id}/answers
- POST /practice-sessions/{id}:finish
- GET /practice-sessions/{id}/result

## Criterios de aceptacion
- Guarda respuestas.
- Calcula score y errores frecuentes.
- Bloquea envio de respuestas tras finalizar (409).

## Tests de integracion
- StartSession_Returns201
- SubmitAnswers_AfterFinish_Returns409
- FinishSession_ComputesScoreAndWeakPoints

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 9) US-007 - Feedback del padre sobre dificultad

- **Title:** `US-007 Parent feedback and lesson difficulty tuning`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Historia
Como padre/madre quiero valorar la dificultad para ajustar proximas lecciones.

## Endpoint
- POST /lessons/{id}/parent-feedback

## Criterios de aceptacion
- Acepta `too_easy`, `adequate`, `too_hard`.
- Persiste feedback.
- Ajusta dificultad siguiente leccion (+1 / 0 / -1).

## Tests de integracion
- SubmitFeedback_AdjustsNextLessonDifficulty
- SubmitFeedback_InvalidValue_Returns400
- SubmitFeedback_LessonNotFound_Returns404

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 10) US-008 - Progreso agregado del alumno

- **Title:** `US-008 Student progress overview endpoint`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:data`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Historia
Como padre/madre quiero ver un resumen de progreso para tomar decisiones.

## Endpoint
- GET /students/{id}/progress/overview

## Criterios de aceptacion
- Devuelve nivel actual, sesiones completadas, media ultimas 5 y foco recomendado.
- Soporta estado sin sesiones previas.
- 404 para alumno inexistente.

## Tests de integracion
- GetOverview_ReturnsAggregatedMetrics
- GetOverview_StudentNotFound_Returns404
- GetOverview_NoSessions_ReturnsEmptyState

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 11) INF-003 - Docker local para desarrollo y tests

- **Title:** `INF-003 Dockerized local environment (API + PostgreSQL)`
- **Labels:** `type:infra`, `priority:p0`, `area:devops`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Contexto
Necesitamos reproducibilidad local para onboarding y demo.

## Alcance
- Dockerfile para API.
- docker-compose con API + PostgreSQL.
- Variables de entorno y volumenes de datos.

## Criterios de aceptacion
- `docker compose up` deja API funcional en local.
- README actualizado con pasos de arranque.

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 12) INF-004 - CI baseline

- **Title:** `INF-004 CI baseline: build + unit + integration tests`
- **Labels:** `type:infra`, `type:testing`, `priority:p0`, `area:devops`, `area:quality`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Contexto
Todo cambio debe validarse automaticamente para mantener calidad.

## Alcance
- GitHub Actions pipeline:
  - restore/build
  - unit tests
  - integration tests con contenedores
- Cache de paquetes NuGet.
- Estado de checks requerido para PR.

## Criterios de aceptacion
- Pipeline verde en rama principal.
- Falla si falla cualquier test.
- Tiempo de pipeline razonable (<10 min objetivo inicial).

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

### 13) INF-005 - Documentacion tecnica para portfolio

- **Title:** `INF-005 Technical README and testing strategy`
- **Labels:** `type:infra`, `priority:p0`, `area:quality`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Contexto
El proyecto debe comunicar valor tecnico profesional en GitHub.

## Alcance
- README principal con:
  - problema y propuesta de valor
  - arquitectura
  - como ejecutar local
  - roadmap
- `docs/testing/TESTING_STRATEGY.md`:
  - test pyramid aplicada al proyecto
  - que va en unit, integration, api-contract
  - uso de test doubles

## Criterios de aceptacion
- Cualquier persona puede levantar el proyecto y ejecutar tests siguiendo README.
- Se entiende por que el proyecto prioriza integration testing.

## Definition of Done
- Endpoint o modulo documentado en OpenAPI o README tecnico.
- Migraciones EF Core al dia y aplicadas en entorno de test.
- 1+ integration test happy-path.
- 2+ integration tests negativos (cuando aplique).
- Reglas de dominio cubiertas con unit tests.
- Logs estructurados en inicio/fin de caso de uso y errores.
- CI en verde.
```

## Orden de ejecucion recomendado

1. INF-001
2. INF-002
3. US-001
4. US-002
5. US-003
6. US-004
7. US-005
8. US-006
9. US-007
10. US-008
11. INF-003
12. INF-004
13. INF-005
