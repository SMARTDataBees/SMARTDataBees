#region Header
// Revit MEP API sample application
//
// Copyright (C) 2007-2015 by Jeremy Tammik, Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software
// for any purpose and without fee is hereby granted, provided
// that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  
// AUTODESK, INC. DOES NOT WARRANT THAT THE OPERATION OF THE 
// PROGRAM WILL BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject
// to restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
#endregion // Header

#region Namespaces

using System;
using System.Windows.Forms;
using SDBees.GuiTools;

#endregion // Namespaces

namespace AdnRme
{
  public partial class ProgressForm : Form, iProgress
  {
    string _format;

    /// <summary>
    /// Set up progress bar form and immediately display it modelessly.
    /// </summary>
    /// <param name="parentWindowHandle">Parent window handle</param>
    /// <param name="caption">Form caption</param>
    /// <param name="format">Progress message string</param>
    /// <param name="max">Number of elements to process</param>
    public ProgressForm( IntPtr parentWindowHandle, string caption, string format, int max )
    {
        _format = format;
        InitializeComponent();
        Text = caption;
        label1.Text = (null == format) ? caption : string.Format(format, 0);
        progressBar1.Minimum = 0;
        progressBar1.Maximum = max;
        progressBar1.Value = 0;
        if (parentWindowHandle == null)
            Show();
        else
            Show(new JtWindowHandle(parentWindowHandle));
        Application.DoEvents();
    }

    /// <summary>
    /// Set up progress bar form and immediately display it modelessly.
    /// </summary>
    /// <param name="caption">Form caption</param>
    /// <param name="format">Progress message string</param>
    /// <param name="max">Number of elements to process</param>
    public ProgressForm(string caption, string format, int max)
    {
        _format = format;
        InitializeComponent();
        Text = caption;
        label1.Text = (null == format) ? caption : string.Format(format, 0);
        progressBar1.Minimum = 0;
        progressBar1.Maximum = max;
        progressBar1.Value = 0;
        Show();
        Application.DoEvents();
    }

#if true //USE_RALFS_PROGRESS_FORM
    public ProgressForm(IntPtr parentWindowHandle)
    {
        var caption = " "; string format = null; var max = 100;

        _format = format;
        InitializeComponent();
        Text = caption;
        label1.Text = (null == format) ? caption : string.Format(format, 0);
        progressBar1.Minimum = 0;
        progressBar1.Maximum = max;
        progressBar1.Value = 0;
        if (parentWindowHandle == null)
            Show();
        else
            Show(new JtWindowHandle(parentWindowHandle));
        CenterToParent();
        Application.DoEvents();
    }

    private string m_text1 = "";

    private string m_text2 = "";
    
    private int m_max;

    public void Set(string caption, int max, string text1, string text2)
    {
        Text = caption;

        m_text1 = text1;

        m_text2 = text2;

        m_max = max;

        progressBar1.Value = 0;

        progressBar1.Maximum = max;

        Update();
    }

    public void SetText(string caption, string text1, string text2)
    {
        Text = caption;

        m_text1 = text1;

        m_text2 = text2;

        Update();
    }

    public void SetMax(int max)
    {
        m_max = max;

        progressBar1.Value = 0;

        progressBar1.Maximum = max;

        Update();
    }

    private void Update()
    {
        if (m_max == 0)
        {
            _format = m_text1;

            label1.Text = string.Format(_format);
        }
        else
        {
            _format = m_text2;

            label1.Text = string.Format(_format, progressBar1.Value, m_max);
        }

        Application.DoEvents();
    }
#endif

    public void Increment(int value = 1)
    {
      progressBar1.Value += value;

      if( null != _format )
      {
        label1.Text = string.Format( _format, progressBar1.Value, m_max);
      }
      
      //Application.DoEvents();

      label1.Update();
    }

    public void IncrementBy(int value)
    {
        Increment(value);
    }

    public void IncrementTo(int value)
    {
        Increment(value - progressBar1.Value);
    }

    public class JtWindowHandle : IWin32Window
    {
        IntPtr _hwnd;

        public JtWindowHandle(IntPtr h)
        {
            _hwnd = h;
        }

        public IntPtr Handle
        {
            get
            {
                return _hwnd;
            }
        }
    }
#if USE_MARTINS_PROGRESS_FORM
    public void SetText(string text)
    {
      label1.Text = text;
      System.Windows.Forms.Application.DoEvents();
    }

    public void SetProgressBarMinMax(int min, int max)
    {
      progressBar1.Minimum = min;
      progressBar1.Maximum = max;
      progressBar1.Value = 0;
    }

    public void IncrementProgressBar()
    {
      progressBar1.Value++;
      System.Windows.Forms.Application.DoEvents();
    }

    public void HideProgressBar()
    {
      progressBar1.Visible = false;
      System.Windows.Forms.Application.DoEvents();
    }

    public void ShowProgressBar()
    {
      progressBar1.Visible = true;
      System.Windows.Forms.Application.DoEvents();
    }
#endif // USE_MARTINS_PROGRESS_FORM

  }
}
