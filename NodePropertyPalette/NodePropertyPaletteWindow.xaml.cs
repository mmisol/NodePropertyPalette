﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.Wpf.Extensions;

namespace NodePropertyPalette
{
    /// <summary>
    /// Interaction logic for NodePropertyPaletteWindow.xaml
    /// </summary>
    public partial class NodePropertyPaletteWindow : Window
    {
        private ViewLoadedParams viewLoadedParams;

        private ICommandExecutive commandExecutive;

        private ViewModelCommandExecutive viewModelCommandExecutive;

        /// <summary>
        /// The unique ID for the NodePropertyPalette view extension. 
        /// Used to identify the view extension when sending recordable commands.
        /// </summary>
        private string uniqueId;

        /// <summary>
        /// Since there is no API for height offset comparing to
        /// DynamoWindow height. Define it as static for now.
        /// </summary>
        private static double sidebarHeightOffset = 200;

        /// <summary>
        /// Create the NodePropertyPalette Window
        /// </summary>
        /// <param name="vlp"></param>
        public NodePropertyPaletteWindow(ViewLoadedParams vlp, string id)
        {
            InitializeComponent();
            viewLoadedParams = vlp;
            // Initialize the height of the datagrid in order to make sure
            // vertical scrollbar can be displayed correctly.
            this.NodesTable.Height = vlp.DynamoWindow.Height - sidebarHeightOffset;
            vlp.DynamoWindow.SizeChanged += DynamoWindow_SizeChanged;
            commandExecutive = vlp.CommandExecutive;
            viewModelCommandExecutive = vlp.ViewModelCommandExecutive;
            uniqueId = id;
        }

        private void DynamoWindow_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            // Update the new height of datagrid
            this.NodesTable.Height = e.NewSize.Height - sidebarHeightOffset;
        }

        private void NodesTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get NodeModel(s) that correspond to selected row(s)
            var selectedNodes = new List<NodeModel>();
            foreach (var item in e.AddedItems)
            {
                // Check NodeModel valid before actual selection
                var nodeModel = (item as PropertyPaletteNodeViewModel).NodeModel;
                if (nodeModel != null)
                {
                    selectedNodes.Add(nodeModel);
                }
            }

            if (selectedNodes.Count() > 0)
            {
                // Select
                var command = new DynamoModel.SelectModelCommand(selectedNodes.Select(nm => nm.GUID), ModifierKeys.None);
                commandExecutive.ExecuteCommand(command, uniqueId, "NodePropertyPalette");

                // Focus on selected
                viewModelCommandExecutive.FindByIdCommand(selectedNodes.First().GUID.ToString());
            }
        }

        internal void Dispose()
        {
            viewLoadedParams.DynamoWindow.SizeChanged -= DynamoWindow_SizeChanged;
        }
    }
}
