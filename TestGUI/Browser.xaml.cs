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
using Microsoft.Win32;
using dotGit.Config;

namespace TestGUI
{

    public partial class Browser : Window
    {
        public Browser()
        {
            InitializeComponent();
            m_commits.SelectionChanged += (o, args) => SelectCommit(m_commits.SelectedItem as Commit);
            //m_config_tree.SelectedItemChanged += (o, args) => SelectConfiguration(m_config_tree.SelectedItem);
        }

        Repository m_repository;


        // load
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = m_url_textbox.Text;
            Repository repo = Repository.Open(url);
            m_repository = repo;
            m_branches.ItemsSource = repo.Branches;
            m_tags.ItemsSource = repo.Tags;
            var list = repo.HEAD.Commit.Ancestors.ToList();
            list.Insert(0, repo.HEAD.Commit);
            m_commits.ItemsSource = list;
            m_commits.SelectedIndex = 0;
            ReloadConfiguration();
        }

        private void OnSelectRepository(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            //dlg.CheckPathExists = true;
            if (dlg.ShowDialog() ==  System.Windows.Forms.DialogResult.OK)
            {
                m_url_textbox.Text = dlg.SelectedPath;
            }
        }

        //private void SelectConfiguration(object obj)
        //{
        //    if (obj is Entry)
        //    {
        //        var entry = obj as dotGit.Config.Entry;
        //        m_config_name.Content = entry.FullName;
        //        if (entry.Value != null)
        //            m_config_value.Text = entry.Value;
        //    }
        //}

        private void SelectCommit(Commit commit)
        {
            if (commit == null)
                return;
            m_tree.ItemsSource = commit.Tree.Children;
            //(m_tree.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsExpanded = true;
        }

        private void OnLoadConfiguration(object sender, RoutedEventArgs e)
        {
            ReloadConfiguration();
        }

        private void ReloadConfiguration()
        {
            m_repository.Config.Reload();
            m_config_tree.ItemsSource = null;
            m_config_tree.ItemsSource = m_repository.Config.Sections;
        }

        private void SaveConfiguration()
        {
            m_repository.Config.Save();
            ReloadConfiguration();
        }

        private void OnSaveConfiguration(object sender, RoutedEventArgs e)
        {
            SaveConfiguration();
        }
    }
}
