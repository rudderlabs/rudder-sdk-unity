# Legacy Android dependency inventory

`Plugins/Android` is a standalone Gradle 6.1.1 / AGP 4.0.1 build. The workflow resolves both `sample-kotlin` and `unity-plugin` with JDK 11 and the existing wrapper. The validated inventory contains 231 Maven package/version entries: 38 runtime, 193 development, 120 direct and 111 indirect, with 558 dependency links. All 38 runtime entries belong to the Kotlin sample; they are not a count of shipped SDK dependencies. Wrappers, repositories and dependency versions are unchanged.

The root has ordinary Gradle dependency declarations and a settings file selecting both modules. Dependabot now targets that one Gradle root with the monthly/zero-routine/seven-day policy. The new workflow also makes the Actions root a real updater input. Hosted updater parsing and security-fix generation remain acceptance checks; local Gradle success does not prove either.

Automatic submission remains enabled, but no managed run appeared during the November 2–3 trial. The custom workflow supplies the nested build root and legacy JDK explicitly. The pinned Gradle action is allowlisted and hosted generation passes; actual default-branch submission remains an acceptance step.

Pull requests only generate read-only artifacts. Changes under `Plugins/Android`, workflow changes on `master`, weekly refreshes and manual runs resolve the graph. Only successful runs on `master` submit through a separate write-permission job that does not check out or execute repository code. The checked-in wrapper is executable. The action's default resolution task and existing runtime/development classification are used.

The custom strict-resolution guard and inventory assertions are omitted to avoid maintaining Gradle-internal reflection and an Android error-message exception. Build/action failures remain visible, but the default resolution task can omit unresolved dependencies and still produce a partial snapshot. This is an accepted monitoring limit; a successful run is not proof of complete resolution. Revisit the guard if a concrete missed dependency or repeated incomplete snapshot warrants its maintenance cost.

This graph covers Maven coordinates resolved from source. It does not establish advisory identities for the bundled `annotation-1.1.0.jar`, `gson-2.8.6.jar`, or prebuilt AARs, and does not prove those binaries match current source. Their provenance and Unity package coverage remain separate limitations. The old JCenter repository can also become unavailable; inspect resolution logs and graph changes when dependencies disappear.

After merge, verify both modules and concrete indirect Maven versions in GitHub at the source revision. Verify the hosted Dependabot parser before accepting fix coverage. DAMP activation is separate; this repository is not added to the service by these files.

Rollback: remove the submission workflow and newly added updater entries. Preserve existing alerts and security settings. Old submitted snapshots can persist until superseded or removed.
