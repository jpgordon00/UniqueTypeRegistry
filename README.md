# UniqueTypeRegistry
A design pattern for defining and creating types from singleton-like classes in C#.

## What is this a singleton-type design pattern?
* Instead of defining a constant list of subclasses, this allows you to create new classes that subclass one file and that are automatically apart of a searchable list.
* The name "singleton-type" comes from the obvious fact that in this design patterns there is a singleton list of types.

## Why would I want this?
* It's easier to subclass a single class than to create constant lists for each class you want singleton-types from.
* There are many use cases for a constant list of objects of varying types. In fact, this design pattern is used commonly with enums or constant/static variables. Especially with inheritance, there are many reasons to maintain a list of varying subclasses of a specific type. An example for a basic use case of this design pattern is written below.
```c#
class Mode {
  ...
}
class PreMode : Mode {
  ...
}
class PostMode : Mode {
  ...
}

class Server : {
  List<Mode> Modes = new List<Mode>{new PreMode(), new PostMode()};
  
  // do something with your list of server modes
}
```
## Why would I want this instead of a singleton?
* The use case for creating objects of a certain type given a string is completely different than a singleton; this design pattern is most similar to keeping constant objects of varying types to define a spawnable list of objects.
