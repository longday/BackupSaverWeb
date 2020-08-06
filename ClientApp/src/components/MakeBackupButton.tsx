import React, { useState, useEffect } from 'react'
import {LogTable} from './LogTable';

export default function MakeBackupButton(): JSX.Element{
    
    let [logs, setLogs] = useState([""]);
    let [logTable, setLogTable] = useState(<div>
                                                <LogTable logs={logs}/>
                                           </div>);

    async function onClickedHandler(): Promise<void>{
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

    useEffect(() => {
        setLogTable(<div>
                        <LogTable logs={logs}/>
                    </div>);
    }, [logs]);
    
    return(
    <>
        <div>
            <button id="backup-btn" onClick={onClickedHandler}>Make Backup Now</button>
        </div>
        {logTable}
    </>
    );
}