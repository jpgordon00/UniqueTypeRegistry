# UniqueTypeRegistry
A design pattern for defining and creating types from singleton-like classes in C#.

## Why would I want this?
* There are many use cases for a constant list of objects of varying types. In fact, this design pattern is used commonly with enums or constant/static variables. A solid example for a use-case of this design pattern is below.
```c#
class ServerMode {
  ...
}
```
## Why would I want this instead of a singleton?
* The use case for creating objects of a certain type given a string is completely different than a singleton; this design pattern is most similar to keeping constant objects of varying types to define a spawnable list of objects.
