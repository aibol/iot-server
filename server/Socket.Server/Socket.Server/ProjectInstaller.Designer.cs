namespace Socket.Server
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SocketServerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SocketServerInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // SocketServerProcessInstaller
            // 
            this.SocketServerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SocketServerProcessInstaller.Password = null;
            this.SocketServerProcessInstaller.Username = null;
            // 
            // SocketServerInstaller
            // 
            this.SocketServerInstaller.Description = "This is the main windows service hosting socket server";
            this.SocketServerInstaller.DisplayName = "SocketServer";
            this.SocketServerInstaller.ServiceName = "SocketServerService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SocketServerProcessInstaller,
            this.SocketServerInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SocketServerProcessInstaller;
        private System.ServiceProcess.ServiceInstaller SocketServerInstaller;
    }
}