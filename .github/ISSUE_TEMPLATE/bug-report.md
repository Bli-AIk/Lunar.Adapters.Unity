---
name: Bug report
about: "Report a bug or unexpected behavior in a C# adapter / bridge library"
title: 'bug: [Short description]'
labels: bug
assignees: ''
---

### 🐛 Problem Description
Provide a clear description of the issue. Include:
- Library name and version (NuGet package, project, or local reference)
- Library type / context: Core bridge / Engine integration / Runtime only / Editor only
- Engine name & version (e.g., Unity 2021.3.45f1, Godot 4.3)
- Type of issue: compile-time / runtime / unexpected behavior
- Short code snippet showing the context (optional, but helpful)

---

### 📝 Steps to Reproduce
Provide a minimal, self-contained example:

```csharp
// Minimal reproducible example
using MyLibrary;

class Program
{
    static void Main()
    {
        var obj = new Foo();
        obj.Bar(null); // Example of problematic call
    }
}
````

1. Which **class** and **method** did you call? What **parameters** did you pass?
2. Engine-specific context: Editor mode / Play mode / Target platform / Scripting backend
3. Operating system and .NET runtime version (e.g., Windows 11, .NET 8.0)
4. How did you **install** and **reference** the library? (NuGet, version number, local reference, etc.)
5. If applicable, include steps to reproduce in the Editor (scene setup, prefab configuration, etc.)

---

### ✅ Expected Result

Describe what the correct behavior should be.
Example:

* Method `Bar` should return a non-null string.
* No exception should be thrown.

---

### 📄 Actual Result / Logs

Include any errors, stack traces, or console output:

```text
System.NullReferenceException: Object reference not set to an instance of an object.
   at MyLibrary.Foo.Bar(String input) in Foo.cs:line 42
```

* Attach Editor log / Player log if available.
* Include screenshots of errors in Editor if relevant.

---

### ⚙ Additional Information

* Does the issue occur in Update / FixedUpdate / Coroutine / Async callback?
* Is the library used in a multi-threaded environment?
* Are there other libraries that might conflict?
* Does the issue appear on specific hardware or OS versions?
* Any workarounds tried?