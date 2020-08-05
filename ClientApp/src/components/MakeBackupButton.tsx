import React, { useState } from 'react'

export default function MakeBackupButton(): JSX.Element{
    
    let [successed, setSuccessed] = useState(false);

    async function onClickedHandler(): Promise<void>{
        const response = await fetch('backup');

        setSuccessed(response.bodyUsed);
        
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