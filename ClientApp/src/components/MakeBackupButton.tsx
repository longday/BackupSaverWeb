import React from 'react'

export default function MakeBackupButton(): JSX.Element{
    
    const onClickedHandler = () : void =>{
        fetch('backup');
        alert('Бэкапы были успешно созданы!')
    }
    
    return(
    <>
        <button id="backup-btn" onClick={onClickedHandler}>Make Backup Now</button>
    </>
    );
}