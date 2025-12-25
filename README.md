
# This repository contains benchmarks written in C# using BenchmarkDotNet.

## DictionaryBenchmarks: Dictionary Key Choice: `Type` vs Enum

This benchmark compares dictionary lookup performance when using
`System.Type` keys versus enum keys to identify character types.
The goal is to **measure both runtime cost and JIT code generation**
differences in a hot lookup path.

The scenario models a common design choice in game and engine-style
code, where behavior or data is selected based on a character’s type.

---

### Benchmark Setup

Two dictionaries with identical logical contents are used:

- `Dictionary<Type, CharacterBase>` keyed by concrete runtime types
- `Dictionary<CharacterType, CharacterBase>` keyed by an enum

Both dictionaries:
- Contain four entries
- Are initialized once and reused
- Perform repeated lookups without allocations

The benchmarked methods perform multiple lookups per invocation to
amplify small per-call differences.

---

### Environment

- OS: Windows 11 (x64)  
- CPU: AMD Ryzen 9 5950X  
- Runtime: .NET 10 (RyuJIT, x64)  
- Tooling: BenchmarkDotNet v0.15.8  
- Diagnosers: MemoryDiagnoser, DisassemblyDiagnoser
  
---


### Results

| Method                     | Mean      | StdDev    | Code Size | Allocated |
|--------------------------- |----------:|----------:|----------:|----------:|
| TypeKeyLookupTest          | 16.224 ns | 0.265 ns  | 3,046 B   | 0 B       |
| CharacterTypeKeyLookupTest |  6.262 ns | 0.121 ns  | 1,379 B   | 0 B       |
---

### Analysis

The enum-based lookup is approximately **2.5× faster** than the
Type-based lookup in this scenario.

Key observations:
- **Simpler hashing and equality checks:** Enum keys are backed by integers.
  While `int.GetHashCode()` is technically a virtual method, its implementation
  is extremely simple (`return m_value;`) and the JIT can inline it easily.  
- **Additional indirection for Type keys:** Type-based dictionary keys are references.
  Lookup involves pointer dereferences and virtual calls for `Type.GetHashCode()` and
  `Type.Equals()`. These extra instructions increase the JIT-generated code size
  and slightly slow down lookups.  
- **More aggressive inlining for enum keys:** The JIT can inline operations for
  enum keys more easily because the methods are small and deterministic.  
- **Zero allocations:** Both lookup approaches allocate zero bytes per operation,
  confirmed by `Allocated = 0 B`.
  
---

### Takeaway

Using `System.Type` as a dictionary key is convenient and expressive,
but it carries measurable overhead compared to enum-based keys.
In performance-sensitive code paths, especially those executed
frequently per frame or per request, enum keys provide a significantly
more efficient alternative.
