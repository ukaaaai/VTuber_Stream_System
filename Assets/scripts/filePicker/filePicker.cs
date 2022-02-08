using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Events;
using System;

public class filePicker : MonoBehaviour
{
    [Serializable] class FilePick : UnityEvent<string> { }
    [SerializeField] private FilePick filePick = new FilePick();

    public void OnClick()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "json file(*.json)|*.json";
        openFileDialog.Multiselect = false;
        openFileDialog.FilterIndex = 2;
        openFileDialog.RestoreDirectory = true;

        if(openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string filePath = openFileDialog.FileName;
            Debug.Log(filePath);
            modelManager.AddModel(filePath);
        }
    }
}
