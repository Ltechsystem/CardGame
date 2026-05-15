using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CardGame.ViewModels;

namespace CardGame;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null) return new TextBlock { Text = "No data" };

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(name);
            if (type is not null)
                return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = $"View not found: {name}" };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
