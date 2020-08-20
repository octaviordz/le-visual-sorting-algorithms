using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualSortingAlgorithms.Wpf
{
    public static class SortingAlgorithm
    {
        public static readonly DependencyProperty Name = DependencyProperty.RegisterAttached(nameof(Name),
            typeof(string), typeof(SortingAlgorithm), new FrameworkPropertyMetadata(null));

        public static string GetName(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (string)element.GetValue(Name);
        }
        public static void SetName(UIElement element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(Name, value);
        }
    }
}
