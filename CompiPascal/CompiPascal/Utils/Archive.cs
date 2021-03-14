using System;
using System.IO;
using System.Windows.Forms;

namespace compiler_app
{
    class Archive
    {

        private String textFound = null;
        private String path = null;

        public Archive()
        {

        }

        public Boolean saveAsArchive(SaveFileDialog saveFileDialog, String information)
        {
            try
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {//if press OK

                    if (File.Exists(saveFileDialog.FileName))
                    {//if exists a name
                        this.path = saveFileDialog.FileName;//save the name on the Dialog

                        StreamWriter textToSave = File.CreateText(this.path);

                        textToSave.Write(information);
                        textToSave.Flush();
                        textToSave.Close();
                    }
                    else
                    {
                        this.path = saveFileDialog.FileName;//save the name on the Dialog

                        StreamWriter textToSave = File.CreateText(this.path);

                        textToSave.Write(information);
                        textToSave.Flush();
                        textToSave.Close();
                    }
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("No se pudo guardar el archivo");
                return false;
            }
            return false;
        }

        public Boolean saveArchive(String info)
        {
            try
            {
                StreamWriter textToSave = File.CreateText(this.path);
                textToSave.Write(info);
                textToSave.Flush();
                textToSave.Close();
            }
            catch
            {
                MessageBox.Show("No se pudo guardar");
            }
            return false;
        }

        //----------------GETTER AND SETTER
        public String getTextFound()
        {
            return this.textFound;
        }

        public String getPath()
        {
            return this.path;
        }
    }
}
