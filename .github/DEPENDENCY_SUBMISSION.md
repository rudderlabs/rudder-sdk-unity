# Legacy Android dependency inventory

`Plugins/Android` is a standalone Gradle 6.1.1 / AGP 4.0.1 build. The workflow resolves both `sample-kotlin` and `unity-plugin` with JDK 11. Local resolution generated 231 Maven entries, including runtime and development dependencies. Wrappers, repositories and dependency versions are unchanged.

The root has ordinary Gradle dependency declarations and a settings file selecting both modules. Dependabot now targets that one Gradle root with the monthly/zero-routine/seven-day policy. The new workflow also makes the Actions root a real updater input. Hosted updater parsing and security-fix generation remain acceptance checks; local Gradle success does not prove either.

Pull requests only generate read-only artifacts. Dependency changes on `master`, weekly refreshes and manual runs resolve the graph. Only successful runs on `master` submit through a separate write-permission job. Unresolved dependencies fail graph generation. One known exception is Kotlin metadata in `sample-kotlin`, which cannot choose a debug or release variant of the local `unity-plugin` project. The plugin runtime and test configurations are checked separately; other unresolved dependencies fail the run. The Gradle action needs organization allowlist approval before hosted execution.

This graph covers Maven coordinates resolved from source. It does not establish advisory identities for the bundled `annotation-1.1.0.jar`, `gson-2.8.6.jar`, or prebuilt AARs, and does not prove those binaries match current source. Their provenance and Unity package coverage remain separate limitations. The old JCenter repository can also become unavailable; resolution failures must remain visible.

After merge, verify both modules and concrete indirect Maven versions in GitHub at the source revision. Verify the hosted Dependabot parser before accepting fix coverage. DAMP activation is separate; this repository is not added to the service by these files.

Rollback: remove the submission workflow and newly added updater entries. Preserve existing alerts and security settings. Old submitted snapshots can persist until superseded or removed.
