using DBMoviesManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBMoviesManager
{
    public enum ROLE {ActorROLE,DirectorROLE,MovieROLE}
    public partial class ChoiceFromDuplicationWindow : Window
    {
        internal ROLE role;
        public Actor actor = null;
        public Director director = null;
        public Movie movie = null;


        public ChoiceFromDuplicationWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (role) {
                case (ROLE.ActorROLE): actor = lsChoices.SelectedItem as Actor;
                        break;
                case (ROLE.DirectorROLE):director = lsChoices.SelectedItem as Director; 
                    break;
                case (ROLE.MovieROLE):
                    movie = lsChoices.SelectedItem as Movie;
                    break;
            }
            Close();
        }
    }
}
