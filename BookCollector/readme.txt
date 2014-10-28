Next steps
1. Update app.xaml
2. Add a Shell (derived from IShell or ShellBase)

--- App.xaml - example ---

<Application x:Class="<project>.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:core="clr-namespace:Framework.Core;assembly=Framework">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml" />
                <ResourceDictionary>
                    <core:ApplicationBootstrapper x:Key="Bootstrapper"/>
                </ResourceDictionary>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>

--- Shell ViewModel - example ---

--- Shell View - example ---

<controls:MetroWindow x:Class="BookCollector.Shell.ShellView"
                      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                      xmlns:controls="http://metro.mahapps.com/winfx/xaml/controls"
                      xmlns:cal="http://www.caliburnproject.org"
                      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
                      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                      xmlns:core="clr-namespace:Framework.Core;assembly=Framework.Core"
                      mc:Ignorable="d" 
                      Title="Book Collector" Height="1000" Width="1200">
    
    <controls:MetroWindow.LeftWindowCommands>
        <controls:WindowCommands>
            <ItemsControl x:Name="LeftShellCommands">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <StackPanel Orientation="Horizontal"/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <Button x:Name="Execute"
                                Content="{Binding DisplayName}"
                                cal:Bind.Model="{Binding}"/>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
        </controls:WindowCommands>
    </controls:MetroWindow.LeftWindowCommands>

    <controls:MetroWindow.RightWindowCommands>
        <controls:WindowCommands>
            <ItemsControl x:Name="RightShellCommands">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <StackPanel Orientation="Horizontal"/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <Button x:Name="Execute"
                                Content="{Binding DisplayName}"
                                cal:Bind.Model="{Binding}"/>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
        </controls:WindowCommands>
    </controls:MetroWindow.RightWindowCommands>

    <controls:MetroWindow.Flyouts>
        <controls:FlyoutsControl x:Name="ShellFlyouts" d:DataContext="{d:DesignInstance core:FlyoutBase}">
            <controls:FlyoutsControl.ItemContainerStyle>
                <Style TargetType="{x:Type controls:Flyout}" BasedOn="{StaticResource {x:Type controls:Flyout}}">
                    <Setter Property="Header" Value="{Binding DisplayName}" />
                    <Setter Property="IsOpen" Value="{Binding IsOpen}" />
                    <Setter Property="IsPinned" Value="{Binding IsPinned}"/>
                    <Setter Property="Position" Value="{Binding Position}" />
                </Style>
            </controls:FlyoutsControl.ItemContainerStyle>
        </controls:FlyoutsControl>
    </controls:MetroWindow.Flyouts>

    <!--<controls:MetroContentControl x:Name="Content"/>-->
</controls:MetroWindow>
