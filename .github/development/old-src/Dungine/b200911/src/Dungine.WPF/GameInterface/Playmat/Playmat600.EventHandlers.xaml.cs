/* PROJECT: Dungine.WPF (https://github.com/calistadalanegames/Dungine.WPF)
 *    FILE: Dungine.WPF.Playmat.Playmat600.EventHandlers.xaml.cs
 * UPDATED: 11-9-2020-4:26 PM
 * LICENSE: Apache v2 (https://apache.org/licenses/LICENSE-2.0)
 *          Copyright 2020 A Pretty Cool Program & Calistadalane Games
 *          All rights reserved
 */

/*  Event handlers for Dungine.WPF.Playmat.Playmat600.xaml.cs.
 */

using System.Windows;

namespace Dungine.WPF.Playmat
{
    public partial class Playmat600 : Window
    {
        /// <include file='Playmat600.comment.xml' path='class/member[@name="btnGameQuit_Click"]/*'/>
        private void btnGameQuit_Click(object sender, RoutedEventArgs e)
        {
            Du.Core.Terminate.ApplicationEnvironment();
        }
    }
}

/* Authors
 * development @aprettycoolprogram.com
 */