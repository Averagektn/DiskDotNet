using Disk.ViewModel;
using System.Windows;
using System.Windows.Controls;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private SettingsViewModel? ViewModel => DataContext as SettingsViewModel;
        private static Settings Settings => Settings.Default;

        public SettingsView()
        {
            InitializeComponent();

            TbIP.Text = Settings.IP;
            TbPort.Text = Settings.Port.ToString();

            //TbTargetMaxTime.Text = Settings.TARGET_MAX_TIME.ToString();
            //TbTargetMinTime.Text = Settings.TARGET_MIN_TIME.ToString();

            TbMoveTime.Text = Settings.MoveTime.ToString();
            TbShotTime.Text = Settings.ShotTime.ToString();

            //TbUserSpeed.Text = Settings.IniUserSpeed.ToString();
            TbUserRadius.Text = Settings.IniUserRadius.ToString();

            //TbEnemySpeed.Text = Settings.IniEnemySpeed.ToString();
            //TbEnemyRadius.Text = Settings.IniEnemyRadius.ToString();

            TbTargetRadius.Text = Settings.IniTargetRadius.ToString();

/*            TbEnemiesNum.Text = Settings.ENEMIES_NUM.ToString();

            TbUserFileAng.Text = Settings.USER_ANG_LOG_FILE;
            TbUserFileCen.Text = Settings.USER_CEN_LOG_FILE;
            TbUserFileWnd.Text = Settings.USER_WND_LOG_FILE;

            TbEnemyAngLogName.Text = Settings.ENEMY_ANG_LOG_NAME;
            TbEnemyCenLogName.Text = Settings.ENEMY_CEN_LOG_NAME;
            TbEnemyWndLogName.Text = Settings.ENEMY_WND_LOG_NAME;*/

/*            TbLogSeparator.Text = Settings.LOG_SEPARATOR.ToString();
            TbLogExtension.Text = Settings.LOG_EXTENSION;

            TbEnemyLogPath.Text = Settings.ENEMY_LOG_PATH;*/

            Settings.UserColor = Settings.UserColor;
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.IP = TbIP.Text;
                Settings.Port = Convert.ToInt32(TbPort.Text);

                //Settings.TARGET_MAX_TIME = Convert.ToInt32(TbTargetMaxTime.Text);
                //Settings.TARGET_MIN_TIME = Convert.ToInt32(TbTargetMinTime.Text);

                Settings.MoveTime = Convert.ToInt32(TbMoveTime.Text);
                Settings.ShotTime = Convert.ToInt32(TbShotTime.Text);

                //Settings.IniUserSpeed = Convert.ToInt32(TbUserSpeed.Text);
                Settings.IniUserRadius = Convert.ToInt32(TbUserRadius.Text);
/*
                Settings.IniEnemySpeed = Convert.ToInt32(TbEnemySpeed.Text);
                Settings.IniEnemyRadius = Convert.ToInt32(TbEnemyRadius.Text);*/

                Settings.IniTargetRadius = Convert.ToInt32(TbTargetRadius.Text);

/*                Settings.ENEMIES_NUM = Convert.ToInt32(TbEnemiesNum.Text);

                Settings.USER_ANG_LOG_FILE = TbUserFileAng.Text;
                Settings.USER_CEN_LOG_FILE = TbUserFileCen.Text;
                Settings.USER_WND_LOG_FILE = TbUserFileWnd.Text;

                Settings.ENEMY_ANG_LOG_NAME = TbEnemyAngLogName.Text;
                Settings.ENEMY_CEN_LOG_NAME = TbEnemyCenLogName.Text;
                Settings.ENEMY_WND_LOG_NAME = TbEnemyWndLogName.Text;*/

/*                Settings.LOG_SEPARATOR = TbLogSeparator.Text[0];
                Settings.LOG_EXTENSION = TbLogExtension.Text;

                Settings.ENEMY_LOG_PATH = TbEnemyLogPath.Text;*/

                Settings.UserColor = Settings.UserColor;
            }
            catch (FormatException)
            {
                _ = MessageBox.Show("Введено некорректное значение");
            }
            finally
            {
                Settings.Save();
                ViewModel?.Close();
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e) => ViewModel?.Close();
    }
}
