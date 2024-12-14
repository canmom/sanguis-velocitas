# Psyshock Community Engines

This is an addon Unity package for the Latios Framework containing various
physics engines built on Psyshock’s low-level APIs. You can use these physics
engines as-is, as a starting point for your own engine, or merely as a learning
resource.

Engines are developed organically based on the needs and wants of the Latios
Framework community. When there are conflicting tradeoffs between different sets
of users, new engines are born.

As such, if you intend to use an engine and desire new features or changes, you
**MUST SPEAK UP** or else nothing will happen. You can do so on the framework’s
Discord. Additionally, speak up if a proposed change negatively impacts you, as
that can help give rise to new engines. Pull requests are also accepted if you
prefer that route.

## Usage

First install the Latios Framework into the project. The current dependency is
0.11.1.

Next, install this package.

If an engine requires a scripting define symbol, add that to your project.

Each engine comes with a baking bootstrap installer in its authoring namespace,
and a runtime installer in its base namespace. Add these installation commands
to your bootstrap next to the framework installers.

## Engines

This package is designed to contain multiple engines. However, there’s currently
only one engine to start with.

### Anna

Anna is intended to be a beginner’s general-purpose engine.

To use it, use the `CollisionTagAuthoring` component to specify static
environment and kinematic colliders in your scene. And use the built-in
`UnityEngine.Rigidbody` component or the `AnnaRigidBodyAuthoring` component to
set up rigid bodies.
