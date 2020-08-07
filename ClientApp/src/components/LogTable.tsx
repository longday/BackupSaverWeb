import React from "react";

export interface ILog{
    date: Date;
    message: string;
}

interface ILogProps{
    logs: ILog[];
}

export const LogTable: React.FunctionComponent<ILogProps> = (props) =>{

    return(
        <>
            <table>
                <tbody>
                    {props.logs.map(log => <tr>{log.message}</tr>)}
                </tbody>
            </table>
        </>
    );
}