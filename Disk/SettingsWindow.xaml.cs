using System.Windows;

using Settings = Disk.Properties.Config.Config;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private static Settings Settings => Settings.Default;

        /// <summary>
        /// 
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();

            TbIP.Text = Settings.IP;
            TbPort.Text = Settings.PORT.ToString();

            TbTargetMaxTime.Text = Settings.TARGET_MAX_TIME.ToString();
            TbTargetMinTime.Text = Settings.TARGET_MIN_TIME.ToString();

            TbMoveTime.Text = Settings.MOVE_TIME.ToString();
            TbShotTime.Text = Settings.SHOT_TIME.ToString();

            TbUserSpeed.Text = Settings.USER_INI_SPEED.ToString();
            TbUserRadius.Text = Settings.USER_INI_RADIUS.ToString();

            TbEnemySpeed.Text = Settings.ENEMY_INI_SPEED.ToString();
            TbEnemyRadius.Text = Settings.ENEMY_INI_RADIUS.ToString();

            TbTargetRadius.Text = Settings.TARGET_INI_RADIUS.ToString();

            TbEnemiesNum.Text = Settings.ENEMIES_NUM.ToString();

            TbUserFileAng.Text = Settings.USER_ANG_LOG_FILE;
            TbUserFileCen.Text = Settings.USER_CEN_LOG_FILE;
            TbUserFileWnd.Text = Settings.USER_WND_LOG_FILE;

            TbEnemyAngLogName.Text = Settings.ENEMY_ANG_LOG_NAME;
            TbEnemyCenLogName.Text = Settings.ENEMY_CEN_LOG_NAME;
            TbEnemyWndLogName.Text = Settings.ENEMY_WND_LOG_NAME;

            TbLogSeparator.Text = Settings.LOG_SEPARATOR.ToString();
            TbLogExtension.Text = Settings.LOG_EXTENSION;

            TbEnemyLogPath.Text = Settings.ENEMY_LOG_PATH;

            Settings.USER_COLOR = Settings.USER_COLOR;
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.IP = TbIP.Text;
                Settings.PORT = Convert.ToInt32(TbPort.Text);

                Settings.TARGET_MAX_TIME = Convert.ToInt32(TbTargetMaxTime.Text);
                Settings.TARGET_MIN_TIME = Convert.ToInt32(TbTargetMinTime.Text);

                Settings.MOVE_TIME = Convert.ToInt32(TbMoveTime.Text);
                Settings.SHOT_TIME = Convert.ToInt32(TbShotTime.Text);

                Settings.USER_INI_SPEED = Convert.ToInt32(TbUserSpeed.Text);
                Settings.USER_INI_RADIUS = Convert.ToInt32(TbUserRadius.Text);

                Settings.ENEMY_INI_SPEED = Convert.ToInt32(TbEnemySpeed.Text);
                Settings.ENEMY_INI_RADIUS = Convert.ToInt32(TbEnemyRadius.Text);

                Settings.TARGET_INI_RADIUS = Convert.ToInt32(TbTargetRadius.Text);

                Settings.ENEMIES_NUM = Convert.ToInt32(TbEnemiesNum.Text);

                Settings.USER_ANG_LOG_FILE = TbUserFileAng.Text;
                Settings.USER_CEN_LOG_FILE = TbUserFileCen.Text;
                Settings.USER_WND_LOG_FILE = TbUserFileWnd.Text;

                Settings.ENEMY_ANG_LOG_NAME = TbEnemyAngLogName.Text;
                Settings.ENEMY_CEN_LOG_NAME = TbEnemyCenLogName.Text;
                Settings.ENEMY_WND_LOG_NAME = TbEnemyWndLogName.Text;

                Settings.LOG_SEPARATOR = TbLogSeparator.Text[0];
                Settings.LOG_EXTENSION = TbLogExtension.Text;

                Settings.ENEMY_LOG_PATH = TbEnemyLogPath.Text;

                Settings.USER_COLOR = Settings.USER_COLOR;
            }
            catch (FormatException)
            {
                MessageBox.Show("Введено некорректное значение");
            }
            finally
            {
                Settings.Save();
                Close();
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e) => Close();
    }
}
