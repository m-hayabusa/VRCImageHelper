namespace VRCImageHelper.UI;

using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class SendNotify
{
    public static void Send(string message, bool silent = true, OnActivated? onClicked = null, Dictionary<string, string>? args = null)
    {
        var tag = Guid.NewGuid().ToString();
        var builder = new ToastContentBuilder()
             .AddAudio(new ToastAudio() { Silent = silent })
             .AddText(message);

        if (args is not null)
            foreach (var item in args)
            {
                builder.AddArgument(item.Key, item.Value);
            }


        builder.AddArgument("tag", tag);
        builder.Show(toast => { toast.Tag = tag; });


        void Callback(ToastNotificationActivatedEventArgsCompat e)
        {
            if (onClicked is not null && ToastArguments.Parse(e.Argument).Get("tag") == tag)
                onClicked(e);
        }

        if (onClicked is not null)
        {
            ToastNotificationManagerCompat.OnActivated += Callback;
        };


        if (silent)
            new Task(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(30)).Wait();
                ToastNotificationManagerCompat.OnActivated -= Callback;
                ToastNotificationManagerCompat.History.Remove(tag);
            }).Start();
    }

    public static void Cleanup()
    {
        ToastNotificationManagerCompat.History.Clear();
        ToastNotificationManagerCompat.Uninstall();
    }
}
