import React, { useState } from 'react'
import {LogTable} from './LogTable';
import {ILog } from './LogTable'
import './MakeBackupButton.css'
import { Log } from '../Log';

export default function MakeBackupButton(): JSX.Element{
    
    let [logs, setLogs] = useState([new Log(new Date(), "")]);

    async function onMakeBackupClickedHandler(): Promise<void>{
        
        try {
            const response: Response = await fetch('backup');

            const newLogs : ILog[] = await response.json() as ILog[];

            setLogs(newLogs);

            alert('Бэкапы были успешно созданы и сохранены');
            
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