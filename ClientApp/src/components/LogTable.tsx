import React from "react";
import { Log } from "../Log";

interface ILogProps{
    logs: Log[];
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