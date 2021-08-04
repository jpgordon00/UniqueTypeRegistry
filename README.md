# UniqueTypeRegistry
A design pattern and C# implementation for defining and creating types from a constant list of types.

## What is a singleton-type design pattern?
* Instead of defining a constant list of subclasses, this allows you to create new classes that subclass one file and that are automatically apart of a searchable list.
* The name "singleton-type" comes from the obvious fact that in this design patterns there is a singleton list of types.
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

  static Server() {
    List<Mode> Modes = new List<Mode>{new PreMode(), new PostMode()};
  
    // select a mode to use by string
    // search through all the modes
    // do something with subclasses of 'Mode'!
  }
}
```

## Why would I want this?
* It's easier to subclass a single class than to create constant lists for each class you want singleton-types from.
* Because all of the type storing is handled by UniqueTypeRegistry.cs, a single list can store many different unqiue types without compromising on speed. Type-lists are created immidetely upon application start, and all find operations are cached.
* Types can be further seperated into seperate searchable lists. UniqueTypeRegistry contains a virtual property, named 'BaseType', that ensures only matching 'BaseTypes' are matched. For example, classes with BaseType="car" will all be matched together and exclude all BaseTypes="motorcyle".
    - BaseType can be ignored for Find to match all subclasses of UniqueTypeRegistry.
* Not only is there a cached-list of subclasses of a given type, but there's also a function to get a subclass of UniqueTypeRegistry given its classname or Type property.
    -  The "Find" and "FindAll" fuctions return a reference to the same object living in UniqueTypeRegistry.Types.

## The approach using UniqueTypeRegistry
Below is a simple example of using UniqueTypeRegistry.cs to find all subclasses, and to find specific subclasses by string.
```c#
class Mode : UniqueTypeRegistry {
  ...
}
class PreMode : Mode {
  ...
}
class PostMode : Mode {
  ...
}

class Server : {  
  static Server() {
    List<Mode> Modes = UniqueTypeRegistry.FindAll<Mode>(); // list of all subclasses of mode
    
    // find mode by classname
    PreMode mode = (PreMode) UniqueTypeRegistry.Find<Mode>("PreMode");
    PostMode mode = (PreMode) UniqueTypeRegistry.Find<Mode>("PostMode");
  }
}
```

Below is another example, this time modeling weapons in a general game. Tthe programmer can add more subclasses of 'Weapon' and use the new classes by referencing their classname by a string. Often I've had to hard-code the instantation of subclassed objects from serialized data (a network stream, JSON data etc.), and as I add new subclasses, its been cumbersome to remember to update the object instantiations. For example, you may be instantiating a Weapon by name, index or a binary number. Anything you can serialize to a string you can use to instantiate a subclass of Weapon.

note: We use virtual methods here instead of abstract ones so we can use the abstract class as a type parameter in Find and FindAll.
```c#
abstract class Weapon : UniqueTypeRegistry {
  public virtual void OnSelect(){}
  public virtual void OnPutAway(){}
  public virtual void Tick(bool firing){}
}

// a sword
class Sword : Weapon {

  // some example properties that apply to the sword only
  int damage = 10;
  float swingTime = 2.5f;

  public override void OnSelect() {
    // do something
  }
  
  public override void OnPutAway() {
    // do something
  }
  
  public override void Tick(bool firing) {
    // do something
  }
  
}

// a gun
class Gun : Weapon {

  // some example properties that apply to the gun only
  int damage = 10;
  int numBullets = 100;
  float firingTime = 0.5f;

  public override void OnSelect() {
    // do something
  }
  
  public override void OnPutAway() {
    // do something
  }
  
  public override void Tick(bool firing) {
    // do something
  }
  
}

// a player using the weapon
// could be a subclass of MonoBehavior maybe
class Player {

  // this could be changed by the Unity editor
  // this could by sent by a binary stream
  // this could be read by Json
  public string WeaponType = "Sword"; 
  
  // assume this is being ticked
  public void Tick() {
    Weapon w = UniqueTypeRegistry.Find<Weapon>(WeaponType);
    w.Tick(true);
  }
  
  // by using WeaponType, w can invoke OnPutAway and OnSelect too
}
```
Below is an example of how you can use the 'BaseType' property to keep seperate types seperate searchable lists.

Below is an example of how you can define 'Type' properties and how you can search by using strings.

## Unit Tests
Unit tests are coming soon.
