using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace SP_EFT_ProfileEditor
{
    public class MyWorker
    {
        private BackgroundWorker LoadDataWorker;
        private ProgressDialogController progressDialog;
        private List<WorkerTask> Tasks;
        private List<WorkerNotification> workerNotifications;
        public string ErrorTitle { get; set; }
        public string ErrorConfirm { get; set; }

        public MyWorker()
        {
            LoadDataWorker = new BackgroundWorker();
            LoadDataWorker.DoWork += LoadDataWorker_DoWork;
            LoadDataWorker.RunWorkerCompleted += LoadDataWorker_RunWorkerCompleted;
            Tasks = new List<WorkerTask>();
            workerNotifications = new List<WorkerNotification>();
        }

        public async void AddAction(WorkerTask task)
        {
            Tasks.Add(task);
            if (!LoadDataWorker.IsBusy)
            {
                progressDialog = await DialogManager.ShowProgressAsync(Application.Current.MainWindow as MetroWindow, task.Title, task.Description);
                progressDialog.SetIndeterminate();
                LoadDataWorker.RunWorkerAsync();
            }
        }

        private async void LoadDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await progressDialog.CloseAsync();
            if (e.Error != null)
            {
                await DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, ErrorTitle, e.Error.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = ErrorConfirm, AnimateShow = true, AnimateHide = true });
                ExtMethods.Log($"LoadDataWorker | {e.Error.Message}");
            }
            else
            {
                while (workerNotifications.Count > 0)
                {
                    await DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, workerNotifications[0].NotificationTitle, workerNotifications[0].NotificationDescription, MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = ErrorConfirm, AnimateShow = true, AnimateHide = true });
                    workerNotifications.RemoveAt(0);
                }
            }
        }

        private void LoadDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Tasks.Count > 0)
            {
                progressDialog.SetTitle(Tasks[0].Title);
                progressDialog.SetMessage(Tasks[0].Description);
                if (Tasks[0].WorkerNotification != null) workerNotifications.Add(Tasks[0].WorkerNotification);
                Tasks[0].Action();
                Tasks.RemoveAt(0);
            }            
        }
    }

    public class WorkerTask
    {
        public Action Action { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public WorkerNotification WorkerNotification { get; set; }
    }

    public class WorkerNotification
    {
        public string NotificationTitle { get; set; }
        public string NotificationDescription { get; set; }
    }
}