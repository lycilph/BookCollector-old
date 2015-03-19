using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public partial class ImportView
    {
        public ImportView()
        {
            InitializeComponent();

            FlipView.HideControlButtons();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var view_model = DataContext as ImportViewModel;
            if (view_model == null)
                throw new Exception("ViewModel must be of type ImportViewModel");

            foreach (var column in view_model.Columns)
            {
                var grid_view_column = new GridViewColumn { Header = column.Name };

                if (column.UseTemplate)
                {
                    var template = Resources[column.PropertyName + "Template"] as DataTemplate;
                    if (template == null)
                        throw new InvalidDataException(string.Format("Template for {0} must be of type DataTemplate", column.PropertyName));
                    grid_view_column.CellTemplate = template;
                }
                else
                {
                    grid_view_column.DisplayMemberBinding = new Binding(column.PropertyName);
                }

                column.WhenAnyValue(x => x.IsVisible)
                      .Subscribe(visible =>
                      {
                          if (visible)
                              GridView.Columns.Add(grid_view_column);
                          else
                              GridView.Columns.Remove(grid_view_column);
                      });
            }
        }
    }
}
