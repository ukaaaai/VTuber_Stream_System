using System.Windows.Forms;
using System;

public class FilePick
{
    //Singleton
    private FilePick() { }
    private static FilePick _instance = new FilePick();

    public static FilePick getInstance() => _instance;

    public string filePick()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "json file(*.json)|*.json";
        dialog.Multiselect = false;
        dialog.FilterIndex = 2;
        dialog.RestoreDirectory = true;

        if((dialog.ShowDialog() == DialogResult.OK))
        {
            return dialog.FileName;
        }
        else
        {
            throw new Exception("faild");
        }
    }
}
