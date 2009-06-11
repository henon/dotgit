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
using Microsoft.Win32;
using dotGit.Config;
using dotGit.Refs;

namespace TestGUI
{

    public partial class Browser : Window
    {
        public Browser()
        {
            InitializeComponent();
            m_commits.SelectionChanged += (o, args) => SelectCommit(m_commits.SelectedItem as Commit);
            m_branches.SelectionChanged += (o, args) => SelectBranch(m_branches.SelectedItem as Branch);
            m_tags.SelectionChanged += (o, args) => SelectTag(m_tags.SelectedItem as Tag);
            m_tree.SelectedItemChanged += (o, args) => SelectObject(m_tree.SelectedValue as TreeNode);
            //m_config_tree.SelectedItemChanged += (o, args) => SelectConfiguration(m_config_tree.SelectedItem);
        }

        private void  SelectObject(TreeNode node)
        {
            if (node.IsBlob)
            {
                var blob = node as Blob;
                var text = Encoding.UTF8.GetString(blob.Content);
                m_object.Document.Blocks.Clear();
                var p = new Paragraph();
                p.Inlines.Add(text);
                m_object.Document.Blocks.Add(p);
                m_object_title.Text = "Content of " + node.Path;
            }
            else
            {
                m_object.Document.Blocks.Clear();
            }
        }


        Repository m_repository;


        // load
        private void OnLoadRepository(object sender, RoutedEventArgs e)
        {
            var url = m_url_textbox.Text;
            Repository repo = Repository.Open(url);
            m_repository = repo;
            m_branches.ItemsSource = repo.Branches;
            m_tags.ItemsSource = repo.Tags;
            DisplayCommit(repo.HEAD.Commit, "HEAD");
            ReloadConfiguration();
        }

        private void SelectBranch(Branch branch)
        {
            DisplayCommit(branch.Commit, "Branch "+branch.Name);
        }

        private void SelectTag(Tag tag)
        {
            if (tag.Object is Commit)
                DisplayCommit(tag.Object as Commit, "Tag "+tag.Name);
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

        private void DisplayCommit(Commit commit, string info)
        {
            if (commit == null)
                return;
            var list = commit.Ancestors.ToList();
            list.Insert(0, commit);
            m_commits.ItemsSource = list;
            m_commits.SelectedIndex = 0;
            m_commit_title.Text = "Commit history for " + info;
        }

        private void SelectCommit(Commit commit)
        {
            if (commit == null)
                return;
            m_tree.ItemsSource = commit.Tree.Children;
            m_tree_title.Text = "Repository tree of Commit " + commit.SHA;
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
