namespace Ashfur.Utils;


public static class Todo {
    /// <summary>
    /// Always throws NotImplementedException stating that operation is not implemented.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static void TODO() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Always throws NotImplementedException stating that operation is not implemented.
    /// </summary>
    /// <param name="reason">a string explaining why the implementation is missing</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void TODO(string reason) {
        throw new NotImplementedException(reason);
    }
}