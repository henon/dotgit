using System;
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
            m_commits.SelectionChanged += (o, args) => SelectCommit(m_commits.SelectedItem as Commit);
        }

        private void SelectCommit(Commit commit)
        {
            if (commit == null)
                return;
            m_tree.ItemsSource = commit.Tree.Children;
            //(m_tree.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsExpanded = true;
        }

        // load
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = m_url_textbox.Text;
            Repository repo = Repository.Open(url);
            m_branches.ItemsSource = repo.Branches;
            m_tags.ItemsSource = repo.Tags;
            var list = repo.HEAD.Commit.Ancestors.ToList();
            list.Insert(0, repo.HEAD.Commit);
            m_commits.ItemsSource = list;
            m_commits.SelectedIndex = 0;
        }

    }
}
