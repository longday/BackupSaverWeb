import React, { useState } from 'react'
import {LogTable} from './LogTable';

export default function MakeBackupButton(): JSX.Element{
    
    let [logs, setLogs] = useState([""]);

    async function onMakeBackupClickedHandler(): Promise<void>{
        const response: Response = await fetch('backup');

        const newLogs = await response.json();

        setLogs(newLogs);

        if(logs.length > 0)
        {
            alert('Бэкапы были успешно созданы и сохранены');
        }
        else
        {
            alert('Приложение завершило работу с ошибкой')
        }
    }

    return(
    <>
        <div>
            <button onClick={onMakeBackupClickedHandler}>Make Backup Now</button>
        </div>
        <h1>Logs</h1>
        <div>
            <LogTable logs={logs}/>
        </div>
    </>
    );
}