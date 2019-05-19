
## module snippet

Playing with language design, I definitely see that some language constructions can significantly favor the shape of an entire ecosystem and libraries. For example, "static class" in C# introduced late and "using static" even later has not favor a strong usage around it. 

In #starklang for example, I use the term "module" and "import" namespace has the same syntax also for module import (basically similar to F#), which cognitively makes its usage a lot more streamlined and apparent: you can build a comprehensive library with only modules (which then cannot be extended outside your library), you can expose functions along their types, you can have auto-import of modules, functions becomes more first class and easily accessible...

```stark
import core.printf

module my_module {
    public static func print_even(even: int)
                  requires (even & 1) == 0  {
        printfn($"{even} is an even number)")
    }
}
```

- `core`
   - `core.attributes` module
   - `core.diagnostics` module
   - `core.intrinsics` module
   - `core.memory` module
   - `core.sync` module synchronous primitive




