import './App.css';
import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import useInterval from '@use-it/interval';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import Chip from '@mui/material/Chip';
import Box from '@mui/material/Box';
import CheckBoxIcon from '@mui/icons-material/CheckBox';
import CancelPresentationIcon from '@mui/icons-material/CancelPresentation';
import CircularProgress from '@mui/material/CircularProgress';
import { red, green } from '@mui/material/colors';

function App() {
  const [workflow, setWorkflow] = useState(undefined);
  const [delay, setDelay] = useState(1000);
  const [customState, setCustomState] = useState();
  const startSaga = async () => {
    const workflow = await axios.post(`https://disertationdurablesaga.azurewebsites.net/api/SagaClient`, { redirect: 'follow', headers: { 'Content-Type': 'application/json'}});
    setWorkflow(workflow);
  };

  const resetState = async () => {
    setWorkflow(undefined);
    setDelay(1000);
    setCustomState(undefined);
  };

  const refreshCustomState = async () => {
    let state = await axios.get(`${workflow.data.statusQueryGetUri}`, { redirect: 'follow', headers: { 'Content-Type': 'application/json' }});
    setCustomState(state.data.customStatus);
    setDelay(1000);
  };

  useInterval(() => {
    if (workflow && workflow.data && workflow.data.id) {
      console.log('Morcovi Yhaaaa' + JSON.stringify(workflow.data.id));
      refreshCustomState();
    }
  }, delay);

  useEffect(() => {
    console.log('CurrentState---' + customState);
    if (customState === 'Finished' || customState === 'Rolled-Back') {
      setDelay(null);
    }
  }, [customState]);


  return (
    <div>
      <br />
      <Grid container
        direction="row"
        justifyContent="center"
        alignItems="center"
        rowSpacing={4} columnSpacing={4}>
        <Grid item xs>
          <Grid container
            direction="column"
            justifyContent="center"
            alignItems="center"
            rowSpacing={4} columnSpacing={4}>

            <Grid item xs>
              <Button variant="contained"
                disabled={workflow !== undefined}
                onClick={startSaga}>
                Start Saga
      </Button>
            </Grid>
            <Grid item xs>
              <Button variant="contained"
                disabled={delay !== null}
                onClick={resetState}>
                Reset
  </Button>
            </Grid>
            <Grid item xs>
            </Grid>
            <Grid item xs={3} justifyContent="center">
              <Chip label="Hotel" variant="outlined" />
              {(customState === "BookedHotel" || customState === "Finished" || customState === "BookedTaxi") && (<CheckBoxIcon sx={{ color: green[500] }}></CheckBoxIcon>)}
              {(customState === 'Started' || customState === 'TaxiFailure' || customState === 'FlightFailure') && (<CircularProgress />)}
              {customState === 'Rolled-Back' && (<CancelPresentationIcon sx={{ color: red[500] }} />)}
            </Grid>
            <Grid item xs={3}>
              <Chip label="Taxi" variant="outlined" />
              {(customState === "Finished" || customState === "BookedTaxi") && (<CheckBoxIcon sx={{ color: green[500] }}></CheckBoxIcon>)}
              {(customState === 'Started' || customState === "BookedHotel" || customState === 'FlightFailure') && (<CircularProgress />)}
              {(customState === 'TaxiFailure' || customState === 'Rolled-Back') && (<CancelPresentationIcon sx={{ color: red[500] }} />)}
            </Grid>
            <Grid item xs={3}>
              <Chip label="Flight" variant="outlined" />
              {(customState === "Finished") && (<CheckBoxIcon sx={{ color: green[500] }}></CheckBoxIcon>)}
              {(customState === 'Started' || customState === "BookedHotel" || customState === "BookedTaxi") && (<CircularProgress />)}
              {(customState === 'FlightFailure' || customState === 'Rolled-Back') && (<CancelPresentationIcon sx={{ color: red[500] }} />)}
            </Grid>
            <Grid item xs={3}>
            </Grid>
            <Grid item xs={3}>
              {customState && (
                <Chip label={customState} variant="outlined" />)
              }
            </Grid>
            <Grid item xs={3}>
            </Grid>
          </Grid>
        </Grid>
        <Grid item xs>
          <Box
            component="img"
            sx={{
              height: 566,
              width: 1036,
              maxHeight: 566,
              maxWidth: 1036,
            }}
            alt="Saga Background"
            src="saga_background.png" />
        </Grid>
      </Grid>
    </div>
  );
}

export default App;
