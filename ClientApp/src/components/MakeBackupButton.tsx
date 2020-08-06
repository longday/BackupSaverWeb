import React, { useState } from 'react'

export default function MakeBackupButton(): JSX.Element{
    
    let [successed, setSuccessed] = useState(true);

    async function onClickedHandler(): Promise<void>{
        const response: Response = await fetch('backup');

        setSuccessed(response.json() as unknown as boolean);
        
        if(successed)
        {
            alert('Бэкапы были успешно созданы!');
        }
        else
        {
            alert('Произошла ошибка!');
        }
    }
    
    return(
    <>
        <div>
            <button id="backup-btn" onClick={onClickedHandler}>Make Backup Now</button>
        </div>
    </>
    );
}