using System;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using OpenAI_API;
using Elasticsearch.Net;
using Nest;
using System.Runtime.InteropServices;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Net;
using System.IO;
using Serilog;
using Serilog.Core;
using System.Configuration;
using OpenAI_API.Moderation;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CustomControls.Controls;
using System.DirectoryServices.ActiveDirectory;
using WinFormsApp3.Models;
using System.Speech.Synthesis;
using WinFormsApp3.models;
using Microsoft.Extensions.Logging;
using log4net.Repository.Hierarchy;

namespace WinFormsApp3
{
    
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);
        private readonly SearchService searchService;
        private Point offset;
        public int force;
        // Declare class-level variables to store the button colors
        private Color imgBtnColor;
        private Color txtBtnColor;
        private bool hard_btn_clicked = false;
        private int temp_force;

        public SpeechSynthesizer obj;
        
        public Form1()
        {
           
            try
            {
                // Retrieve the API key from app.config
                string? apiKey = ConfigurationManager.AppSettings["ApiKey"];

                InitializeComponent();
                InitializeComponents();
                top_panel.BackColor = Color.FromArgb(125, Color.Black);
                bot_panel.BackColor = Color.FromArgb(125, Color.Black);
                result_panel.BackColor = Color.FromArgb(125, Color.Black);
                results.Visible = false;
                img_results.Visible = false;
                loadingBox.Visible = false;
                Hard_search.Visible = false;
                obj = new SpeechSynthesizer();
                // Set the initial button colors
                imgBtnColor = Color.FromArgb(64, 64, 64, 125);
                txtBtnColor = Color.FromArgb(64, 64, 64, 125);
                force = 0;
                temp_force = 0;

                // Set the toggle button to be on by default
                togglelMode.Checked = true;
                AttachEventHandlers();

                // Check if the apiKey is null before using it
                if (!string.IsNullOrEmpty(apiKey))
                {
                    // Initialize the SearchService with your API key
                    searchService = new SearchService(apiKey);
                }

                prompt_query.KeyDown += Prompt_query_KeyDown;
                results.GotFocus += Results_GotFocus;
                Log.Information("The Form1 Class has been processed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while initializing the application: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Information(ex, "Some error occured while processing form1.");                 
            }
        }

        private void Results_GotFocus(object? sender, EventArgs e)
        {
            if (results.ReadOnly)
            {
                HideCaret(results.Handle);
            }
        }

        // Search on button click
        private async void Btn_search_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Information("Search Button Has been Pressed.");
                force = 0;
                await PerformSearch(hard_btn_clicked, force);

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while performing the search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Information("Some Error has been occured after button press.");// Log or handle the exception as required
            }
        }

        // Search on pressing enter
        private async void Prompt_query_KeyDown(object? sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Log.Information("Prompt Query.");
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    force = 0;
                    await PerformSearch(hard_btn_clicked, force);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while performing the search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Log or handle the exception as required\
                Log.Information(ex, "Some error occured while processing Prompt Query.");
            }
        }
        // hard_search
        private async void Hard_search_Click(object sender, EventArgs e)
        {
            try
            {
                hard_btn_clicked = true; // Set hard_btn_clicked to true when the button is clicked
                if (hard_btn_clicked) { temp_force = force; }
                else
                { force = 0; temp_force = 0; }
                await PerformSearch(hard_btn_clicked, temp_force); // Pass the current force value as newResult parameter
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while performing the search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                hard_btn_clicked = false; // Reset hard_btn_clicked to false
                force = 0;
                temp_force = 0;
            }
        }
        // Function for text search
        private async Task TextSearch(string query, string str, bool? newResult)
        {
            try
            {
                Log.Information($"Text Search query: {query}");
                if (str == "Image")
                {
                    query = models.ImgDataset.ImageProcessing.RemoveImageRelatedWords(query);
                }
                results.Clear();
                string? generatedText = "";
                bool fileExists = await searchService.CheckFileExistsInContainer(query);//checkpoint 1
                bool hard_hit = !(newResult ?? false);
                if (hard_hit && fileExists)
                {
                    if (searchService.PossibleMatch == null)
                    {
                        MessageBox.Show("Error: PossibleMatch is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Return or handle the error condition appropriately
                    }
                    generatedText = await searchService.RetrieveTextFromBlob(query);
                }
                else
                {
                    generatedText = await searchService.PerformTextSearch(query);
                }
                if (generatedText == null) { MessageBox.Show("Invalid Query, try again..."); ClearFields(); }
                else
                {
                    results.Text = generatedText;
                    loadingBox.Visible = false;
                    results.Visible = true;
                    ReadButton.Visible = true;
                    results.ScrollBars = ScrollBars.Vertical;
                    txtBtn.Visible = true;
                    imgBtn.Visible = true;
                }
                txtBtn.BackColor = Color.MediumSlateBlue;
                imgBtn.BackColor = imgBtnColor;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while performing the search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Information(ex, "Some error occured during Text Search.");
            }

        }

        private async Task ImageSearch(string query, string str, bool? newResult)
        {
            try
            {
                Log.Information($"ImageSearch: {query}");
                results.Visible = false;
                ReadButton.Visible = false;

                Image? image;
                bool fileExists = await searchService.CheckFileExistsInContainerImage(query);
                bool hard_hit = !(newResult ?? false);
                double similar = searchService.highestSimilarity;
                if (similar == 100)
                    hard_hit = true;
                if (similar < 100 && similar != 0)
                {
                    Hard_search.Visible = true;
                }
                else { Hard_search.Visible = false; }
                if (hard_hit && fileExists)
                {
                    image = await searchService.RetrieveImageFromBlob(query);
                }
                else
                {
                    image = await searchService.PerformImageSearch(query);
                }


                if (image == null) { MessageBox.Show("Invalid Query, try again!"); ClearFields(); }
                else
                {
                    img_results.Image = image;
                    loadingBox.Visible = false;

                    img_results.Visible = true;
                    img_results.SizeMode = PictureBoxSizeMode.StretchImage;
                    txtBtn.Visible = true;
                    imgBtn.Visible = true;
                }
                imgBtn.BackColor = Color.MediumSlateBlue;
                txtBtn.BackColor = txtBtnColor;
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error occurred while performing the search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Information(ex, "Some error occured during Image Search.");
            }
        }
        // Function to perform the search
        private async Task PerformSearch(bool? newResult = false, int? f = null)
        {
            try
            {
                Log.Information($"{nameof(PerformSearch)}");
                loadingBox.Visible = true;
                string query = prompt_query.Text;
                img_results.Visible = false;
                ReadButton.Visible = false;
                results.Visible = false;
                if (f.HasValue)
                {
                    force = f.Value; // Set the force value only if it has a value
                }

                // Load sample data
                var sampleData = new MLModel1.ModelInput()
                {
                    Content = query,
                };

                // Load model and predict output
                var result = MLModel1.Predict(sampleData);

                string str = result.PredictedLabel;


                if (force == 1 || (force == 0 && str != "Image"))
                {
                    await TextSearch(query, str, newResult);

                }
                else if (force == 2 || (force == 0 && str == "Image"))
                {
                    await ImageSearch(query, str, newResult);
                }

                loadingBox.Visible = false;
                //force = 0;
            }
            catch (Exception ex)
            {
                Log.Information($"Error: {ex}");
                throw;
            }
            



        }

        private void InitializeComponents()
        {
            txtBtn.Visible = false;
            imgBtn.Visible = false;
            ReadButton.Visible = false;
        }

        private void AttachEventHandlers()
        {
            // Attach event handlers for dragging the form
            top_panel.MouseDown += TopPanel_MouseDown;
            top_panel.MouseMove += TopPanel_MouseMove;
            top_panel.MouseUp += TopPanel_MouseUp;
        }

        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Capture the offset between the mouse position and the form's location
                offset = new Point(e.X, e.Y);
            }
        }

        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the new form location based on the mouse movement
                Point newLocation = this.Location;
                newLocation.X += e.X - offset.X;
                newLocation.Y += e.Y - offset.Y;

                // Update the form's location
                this.Location = newLocation;
            }
        }

        private void TopPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the captured mouse
                top_panel.Capture = false;
            }
        }

        private void Close_click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void prompt_query_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Log.Information("Drop Down checking.");
                string cloudId = "appgpt.es.centralindia.azure.elastic-cloud.com"; // Replace with your Elastic Cloud Cluster's Cloud ID
                string username = "elastic"; // Replace with your Elasticsearch username
                string password = "rTL8YPLMmyBHlCR4nq3FRz0h"; // Replace with your Elasticsearch password

                var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
                    .BasicAuthentication(username, password);

                var client = new ElasticClient(settings);

                string[] allTopics = searchService.GetAllTopicsInIndexes(client, "text", "image");
                if (allTopics.Length > 0)
                {
                    prompt_query.AutoCompleteCustomSource.AddRange(allTopics);
                }
                else
                {
                    prompt_query.AutoCompleteCustomSource.Clear();
                }
            }
            catch (Exception ex)
            {
                Log.Information("Error in Drop Down List.");
                throw;
            }
        }

        private void results_TextChanged(object sender, EventArgs e)
        {

        }


        private void minimise_Click(object sender, EventArgs e)
        {
            // Minimize the form
            this.WindowState = FormWindowState.Minimized;
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        private void ClearFields()
        {
            results.Visible = false;
            img_results.Visible = false;
            loadingBox.Visible = false;
            txtBtn.Visible = false;
            imgBtn.Visible = false;
            prompt_query.Text = string.Empty;
            ReadButton.Visible = false;
            obj.Pause();
            obj = new SpeechSynthesizer();
            force = 0;
            temp_force = 0;
            hard_btn_clicked = false;
            Hard_search.Visible = false;
        }

        private void togglelMode_CheckedChanged(object sender, EventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;

            if (toggleButton.Checked)
            {
                string imagePath = @"C:\AppGPT\img\bg.jpg";
                Bitmap image = new Bitmap(imagePath);

                // Now you can use the 'image' object as needed, such as setting it as the background image of a form:
                this.BackgroundImage = image;
            }
            else
            {
                string imagePath = @"C:\AppGPT\img\light_bg.jpg";
                Bitmap image = new Bitmap(imagePath);

                // Now you can use the 'image' object as needed, such as setting it as the background image of a form:
                this.BackgroundImage = image;
            }
        }

        private async void imgBtn_Click(object sender, EventArgs e)
        {
            force = 2;
            loadingBox.Visible = true;

            await PerformSearch();

        }

        private async void txtBtn_Click(object sender, EventArgs e)
        {
            force = 1;
            loadingBox.Visible = true;

            await PerformSearch();

        }

        private void ReadButton_Click_1(object sender, EventArgs e)
        {
            if (obj.State == SynthesizerState.Speaking)
            {
                obj.Pause();
            }
            else if (obj.State == SynthesizerState.Paused)
            {
                obj.Resume();
            }
            else
            {
                obj.Volume = 100;
                obj.Rate = 2;
                obj.SelectVoiceByHints(VoiceGender.Female);
                obj.SpeakAsync(results.Text);
            }
        }
    }
}
