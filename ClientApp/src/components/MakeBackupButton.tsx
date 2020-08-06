import React, { useState, useEffect } from 'react'
import {LogTable} from './LogTable';

export default function MakeBackupButton(): JSX.Element{
    
    let [logs, setLogs] = useState(["Hello", "GoodBye"]);
    let [logTable, setLogTable] = useState(<div>
                                                <LogTable logs={logs}/>
                                           </div>);

    async function onClickedHandler(): Promise<void>{
        const response: Response = await fetch('backup');

        const newLogs = await response.json();

        setLogs(newLogs);
    }

    useEffect(() => {
        setLogTable(<div>
                        <LogTable logs={logs}/>
                    </div>);
    });
    
    return(
    <>
        <div>
            <button id="backup-btn" onClick={onClickedHandler}>Make Backup Now</button>
        </div>
        {logTable}
    </>
    );
}