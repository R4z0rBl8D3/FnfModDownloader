﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Net;

namespace FnfModDownloaderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string LinkValue = "";
        public static string NameValue = "";
        public static string DirValue = "";
        public static bool ZipValue = false;
        public static bool SevenZValue = false;
        public static bool Reinstall = false;
        
        public MainWindow()
        {
            InitializeComponent();
            DirTxt.Text = KnownFolders.GetPath(KnownFolder.Downloads) + @"\";
        }

        
        

        private void SetDirDownloads(object sender, RoutedEventArgs e)
        {
            //Set directory text box to downloads
            DirTxt.Text = KnownFolders.GetPath(KnownFolder.Downloads) + @"\";
        }

        private void StartDownload(object sender, RoutedEventArgs e)
        {
            //Check for errors and open download window
            if (DirTxt.Text.EndsWith(@"\"))  {} //Good
            else { DirTxt.Text = DirTxt.Text + @"\"; }
            if (LinkTxt.Text == "")
            {
                MessageBox.Show("A link is required", "Can't start!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (NameTxt.Text == "")
            {
                MessageBox.Show("A name is required", "Can't start!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (NameTxt.Text.Contains(@"\"))
            {
                MessageBox.Show("The name can't contain a backslash", "Can't start!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DirTxt.Text == "")
            {
                MessageBox.Show("A directory is required", "Can't start!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Directory.Exists(DirTxt.Text) == false)
            {
                MessageBox.Show("The directory does not exist", "Can't start!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LinkValue = LinkTxt.Text;
            NameValue = NameTxt.Text;
            DirValue = DirTxt.Text;
            if (ZipRBtn.IsChecked == true) { ZipValue = true; }
            else { ZipValue = false; }
            if (SevenZRBtn.IsChecked == true) { SevenZValue = true; }
            else { SevenZValue = false; }
            DownloadWindow downloadWindow = new DownloadWindow();
            downloadWindow.ShowDialog();
        }

        private void ManageBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageWindow manageWindow = new ManageWindow();
            manageWindow.ShowDialog();
        }

        private void ListBtn_Click(object sender, RoutedEventArgs e)
        {
            ListWindow listWindow = new ListWindow();
            listWindow.ShowDialog();
        }
    }
}
public static class KnownFolders
{
    private static string[] _knownFolderGuids = new string[]
    {
        "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
        "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
        "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
        "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
        "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
        "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
        "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
        "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
        "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
        "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
        "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
    };

    /// <summary>
    /// Gets the current path to the specified known folder as currently configured. This does
    /// not require the folder to be existent.
    /// </summary>
    /// <param name="knownFolder">The known folder which current path will be returned.</param>
    /// <returns>The default path of the known folder.</returns>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
    ///     could not be retrieved.</exception>
    public static string GetPath(KnownFolder knownFolder)
    {
        return GetPath(knownFolder, false);
    }

    /// <summary>
    /// Gets the current path to the specified known folder as currently configured. This does
    /// not require the folder to be existent.
    /// </summary>
    /// <param name="knownFolder">The known folder which current path will be returned.</param>
    /// <param name="defaultUser">Specifies if the paths of the default user (user profile
    ///     template) will be used. This requires administrative rights.</param>
    /// <returns>The default path of the known folder.</returns>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
    ///     could not be retrieved.</exception>
    public static string GetPath(KnownFolder knownFolder, bool defaultUser)
    {
        return GetPath(knownFolder, KnownFolderFlags.DontVerify, defaultUser);
    }

    private static string GetPath(KnownFolder knownFolder, KnownFolderFlags flags,
        bool defaultUser)
    {
        int result = SHGetKnownFolderPath(new Guid(_knownFolderGuids[(int)knownFolder]),
            (uint)flags, new IntPtr(defaultUser ? -1 : 0), out IntPtr outPath);
        if (result >= 0)
        {
            string path = Marshal.PtrToStringUni(outPath);
            Marshal.FreeCoTaskMem(outPath);
            return path;
        }
        else
        {
            throw new ExternalException("Unable to retrieve the known folder path. It may not "
                + "be available on this system.", result);
        }
    }

    [DllImport("Shell32.dll")]
    private static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken,
        out IntPtr ppszPath);

    [Flags]
    private enum KnownFolderFlags : uint
    {
        SimpleIDList = 0x00000100,
        NotParentRelative = 0x00000200,
        DefaultPath = 0x00000400,
        Init = 0x00000800,
        NoAlias = 0x00001000,
        DontUnexpand = 0x00002000,
        DontVerify = 0x00004000,
        Create = 0x00008000,
        NoAppcontainerRedirection = 0x00010000,
        AliasOnly = 0x80000000
    }
}

/// <summary>
/// Standard folders registered with the system. These folders are installed with Windows Vista
/// and later operating systems, and a computer will have only folders appropriate to it
/// installed.
/// </summary>
public enum KnownFolder
{
    Contacts,
    Desktop,
    Documents,
    Downloads,
    Favorites,
    Links,
    Music,
    Pictures,
    SavedGames,
    SavedSearches,
    Videos
}
