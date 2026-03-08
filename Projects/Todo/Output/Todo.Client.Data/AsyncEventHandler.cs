using System.Threading.Tasks;

namespace Todo.Client.Data;

public delegate Task AsyncEventHandler<T>(object sender, T args);
