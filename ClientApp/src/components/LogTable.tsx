import React from "react";

interface ILogProps{
    logs: string[];
}

export const LogTable: React.SFC<ILogProps> = (props) =>{

    return(
        <>
            <table>
                <tbody>
                    {props.logs.map(log => <tr>{log}</tr>)}
                </tbody>
            </table>
        </>
    );
}