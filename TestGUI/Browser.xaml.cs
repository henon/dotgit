﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using dotGit;
using dotGit.Objects;
using System.Collections.ObjectModel;

namespace TestGUI
{

    public partial class Browser : Window
    {
        public Browser()
        {
            InitializeComponent();
            m_list.SelectionChanged += (o, args) => SelectCommit(m_list.SelectedItem as Commit);
        }

        private void SelectCommit(Commit commit)
        {
            m_tree.ItemsSource = commit.Tree.Children;
            //(m_tree.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsExpanded = true;
        }

        // load
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = m_url_textbox.Text;
            Repository repo = Repository.Open(url);
            var list = repo.HEAD.Commit.Ancestors.ToList();
            list.Insert(0, repo.HEAD.Commit);
            m_list.ItemsSource = list;
            m_list.SelectedIndex = 0;
        }

    }
}
