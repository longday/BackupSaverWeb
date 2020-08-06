import React, { useState, useEffect } from "react";

interface ILogProps{
    logs: string[];
}

export const LogTable: React.SFC<ILogProps> = (props) =>{

    let [table, setTable] = useState(<table>
                                        <tbody>
                                            {props.logs.map(log => <tr>{log}</tr>)}
                                        </tbody>
                                    </table>);

    const onClickedHandler = () =>{
        const clearedTable = <table></table>
        
        setTable(clearedTable);
    }

    useEffect(() => {
        setTable(<table>
            <tbody>
                {props.logs.map(log => <tr>{log}</tr>)}
            </tbody>
        </table>);
    });
    
    return(
        <>
            <h1>Logs</h1>
            {table}
            <div>
                <button onClick={onClickedHandler}>Clear logs</button>
            </div>
        </>
    );
}