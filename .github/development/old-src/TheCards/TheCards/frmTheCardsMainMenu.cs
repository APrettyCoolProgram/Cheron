// The Cards main menu form [151122]

using System;
using System.Windows.Forms;

namespace TheCards
{
  public partial class frmTheCardsMainMenu : Form
  {
    /// <summary>Initialize the main menu form </summary>
    public frmTheCardsMainMenu()
    {
      InitializeComponent();
    }

    /// <summary>User clicks the exit button.</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>
    /// Quits the application!
    /// </remarks>
    private void bntExit_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    /// <summary>User clicks the viewer button</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>
    /// Click the viewer!
    /// </remarks>
    private void btnViewer_Click(object sender, EventArgs e)
    {
      frmCardViewer cardViewerWindow = new frmCardViewer();
      this.Hide();
      cardViewerWindow.Show();
    }
  }
}