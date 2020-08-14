import React, { useState } from 'react'
import {LogTable} from './LogTable';
import './MakeBackupButton.css'
import { Log } from '../Log';

export default function MakeBackupButton(): JSX.Element{
    
    let [logs, setLogs] = useState([new Log(new Date(), "")]);

    async function onMakeBackupClickedHandler(): Promise<void>{
        
        try {
            const response: Response = await fetch('backup');

            const data: Log[] = await response.json() as Log[];
            let newLogs: Log[] = data;

            if (logs.length > 1) {
                let tmpData : Log[] = data.filter(item =>
                    logs.every(log => log.date != item.date && log.message != item.message));
                
                newLogs = tmpData.concat(logs).sort((prevLog, currentLog) =>
                    prevLog.date.valueOf() - currentLog.date.valueOf());
            }

            setLogs(newLogs);
        } catch (error) {
            alert('Приложение завершило работу с ошибкой: ' + error);
        }
    }

    return(
    <>
        <div>
            <button onClick={onMakeBackupClickedHandler}>Make Backup Now</button>
        </div>
        <h1>Logs</h1>
        <div className="logTableDiv">
            <LogTable logs={logs}/>
        </div>
    </>
    );
}