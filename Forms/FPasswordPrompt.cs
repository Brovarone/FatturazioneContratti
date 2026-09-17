using System.Windows.Forms;

namespace FatturazioneContratti.Forms
{
    public partial class FPasswordPrompt : Form
    {
        public string Password => txtPsw.Text;

        public FPasswordPrompt()
        {
            InitializeComponent();
        }

        // Metodo helper statico per richiamare la maschera con una sola riga di codice
        public static bool ChiediPassword(IWin32Window owner, string passwordAttesa)
        {
            using var dlg = new FPasswordPrompt();
            if (dlg.ShowDialog(owner) == DialogResult.OK)
            {
                return dlg.Password == passwordAttesa;
            }
            return false;
        }
    }
}