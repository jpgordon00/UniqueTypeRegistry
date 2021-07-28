# UniqueTypeRegistry
A design pattern and C# implementation for defining and creating types a constant list of types.

## What is a singleton-type design pattern?
* Instead of defining a constant list of subclasses, this allows you to create new classes that subclass one file and that are automatically apart of a searchable list.
* The name "singleton-type" comes from the obvious fact that in this design patterns there is a singleton list of types.

## Why would I want this?
* It's easier to subclass a single class than to create constant lists for each class you want singleton-types from.
* Because all of the type storing is handled by UniqueTypeRegistry.cs, a single list can store many different unqiue types without compromising on speed. Type-lists are created immidetely upon application start, and all find operations are cached.
* Types can further seperated into seperate searchable lists. UniqueTypeRegistry contains a virtual property, named 'BaseType', that ensures only matching 'BaseTypes' are matched. For example, classes with BaseType="car" will all be matched together and exclude all BaseTypes="motorcyle".
    - BaseType can be ignored for Find to match all subclasses of UniqueTypeRegistry.
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

