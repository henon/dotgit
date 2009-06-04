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
            m_list.ItemsSource = m_commits;
        }
        ObservableCollection<Commit> m_commits = new ObservableCollection<Commit>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = m_url_textbox.Text;
            Repository repo = Repository.Open(url);
            m_commits.Clear();
            WalkHistory(repo.HEAD.Commit); // Walk history from HEAD
            //WalkHistory(repo.Branches["experimental"].Commit); // Walk history from branch
        }

        private void WalkHistory(Commit commit)
        {
            m_commits.Add(commit);
            //Console.WriteLine("SHA: " + commit.SHA);
            //Console.WriteLine("Committed on: " + commit.CommittedDate);
            //Console.WriteLine("By: " + commit.Committer);

            if (commit.HasParents)
            {
                foreach (Commit parent in commit.Parents)
                    WalkHistory(parent);
            }
        }
    }
}
