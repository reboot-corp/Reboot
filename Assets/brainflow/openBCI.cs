using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using brainflow;
using brainflow.math;

using TMPro;


public class openBCI : MonoBehaviour
{
    private BoardShim board_shim = null;
    private int sampling_rate = 0;
    int[] eeg_channels;
    int nfft;
    public TextMeshProUGUI concentration_lvl_txt;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();
            int board_id = (int)BoardIds.GANGLION_BOARD;
            input_params.serial_port = "COM8";
            board_shim = new BoardShim(board_id, input_params);
            board_shim.prepare_session();
            board_shim.start_stream(450000, "file://brainflow_data.csv:w");
            sampling_rate = BoardShim.get_sampling_rate(board_id);
            Debug.Log("Brainflow streaming was started");
            nfft = DataFilter.get_nearest_power_of_two(sampling_rate);
            eeg_channels = BoardShim.get_eeg_channels(board_shim.get_board_id());
            //print(sampling_rate);
            print("sampling rate :" + sampling_rate);

        }
        catch (BrainFlowException e)
        {
            Debug.Log(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (board_shim == null)
        {
            return;
        }
        int number_of_data_points = sampling_rate * 4;
        double[,] data = board_shim.get_current_board_data(number_of_data_points);
        // check https://brainflow.readthedocs.io/en/stable/index.html for api ref and more code samples
        //Debug.Log("Num elements: " + data.GetLength(1));
        if (gameStartClass.gameStart==true)
        {

            if (gameStartClass.ml == true)
            {
                classifyEEG(data);

            }
            if(gameStartClass.bp == true)
            {
                bandPowerEEG(data);
            }

        }
       



    }

    // you need to call release_session and ensure that all resources correctly released
    private void OnDestroy()
    {
        if (board_shim != null)
        {
            try
            {
                board_shim.release_session();
            }
            catch (BrainFlowException e)
            {
                Debug.Log(e);
            }
            Debug.Log("Brainflow streaming was stopped");
        }
    }



    public void classifyEEG(double[,] data)
    {
        Tuple<double[], double[]> bands = DataFilter.get_avg_band_powers(data, eeg_channels, sampling_rate, true);
        //print(bands.Length);
        //print(bands.Item1.Length);
        double[] feature_vector = bands.Item1.Concatenate(bands.Item2);
        print(feature_vector.Length);
        BrainFlowModelParams model_params = new BrainFlowModelParams((int)BrainFlowMetrics.CONCENTRATION, (int)BrainFlowClassifiers.REGRESSION);
        MLModel concentration = new MLModel(model_params);
        concentration.prepare();
        var concentration_lvl = concentration.predict(feature_vector);
        concentration_lvl_txt.text = ((int)(concentration_lvl * 100f)).ToString() + " %";
        concentrationClass.concentration_lvl = (float)concentration_lvl;
        print("Concentration: " + concentration_lvl);
        concentration.release();
    }

    public void bandPowerEEG(double[,] data)
    {
        int channel = eeg_channels[0]; //Fp1
        //board_shim.release_session();
        double[] detrend = DataFilter.detrend(data.GetRow(channel), (int)DetrendOperations.LINEAR);
        Tuple<double[], double[]> psd = DataFilter.get_psd_welch(detrend, nfft, nfft / 2, sampling_rate, (int)WindowFunctions.HANNING);
        double band_power_alpha = DataFilter.get_band_power(psd, 7.0, 13.0); //alpha filter
        double band_power_beta = DataFilter.get_band_power(psd, 14.0, 30.0); //beta filter
        var band_power = (band_power_alpha / band_power_beta);
        concentrationClass.band_power = (float)band_power;
        concentration_lvl_txt.text = ((int)(band_power * 100f)).ToString() + " %";
        print("Alpha/Beta Ratio:" + (band_power_alpha / band_power_beta));


    }
}